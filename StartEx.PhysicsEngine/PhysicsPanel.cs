using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Reactive;
using DynamicData.Binding;
using StartEx.PhysicsEngine.DataTypes;

namespace StartEx.PhysicsEngine;

public abstract class PhysicsPanel : Canvas {
	public static readonly DirectProperty<PhysicsPanel, IList?> ItemsProperty =
		AvaloniaProperty.RegisterDirect<PhysicsPanel, IList?>(nameof(Items), x => x.items, (x, v) => x.Items = v);

	/// <summary>
	/// 所有项目的列表
	/// </summary>
	public IList? Items {
		get => items;
		set => SetAndRaise(ItemsProperty, ref items, value);
	}

	private IList? items;

	public static readonly DirectProperty<PhysicsPanel, double> ScaleProperty =
		AvaloniaProperty.RegisterDirect<PhysicsPanel, double>(nameof(ScaleProperty), x => x.scale, (x, v) => x.Scale = v);

	/// <summary>
	/// 缩放
	/// </summary>
	public double Scale {
		get => scale;
		set => SetAndRaise(ScaleProperty, ref scale, value);
	}

	private double scale = 128d;

	protected PhysicsPanel() {
		var items = new ObservableCollection<PhysicsObject>();
		items.CollectionChanged += CollectionChanged;
		this.items = items;
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
		base.OnPropertyChanged(change);

		if (change.Property == ItemsProperty) {
			if (change.OldValue is INotifyCollectionChanged oldList) {
				oldList.CollectionChanged -= CollectionChanged;
				CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			}

			if (change.NewValue is INotifyCollectionChanged newList) {
				newList.CollectionChanged += CollectionChanged;
				CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newList as IList));
			}
		}
	}

	private readonly Dictionary<PhysicsObject, IDisposable[]> subscribes = new();

	private void CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
		if (e.NewItems != null) {
			foreach (PhysicsObject newItem in e.NewItems) {
				var control = DataTemplates.FirstOrDefault(t => t.Match(newItem))?.Build(newItem) ?? new Border();
				control.DataContext = newItem;

				if (subscribes.TryGetValue(newItem, out var disposables)) {
					foreach (var disposable in disposables) {
						disposable.Dispose();
					}
				}

				subscribes[newItem] = new[] {
					newItem.WhenPropertyChanged(static p => p.Position)
						.Subscribe(new AnonymousObserver<PropertyValue<PhysicsObject, Vector3>>(value => {
							var v = value.Value;
							var expZ = Math.Exp(v.Z);
							control.SetValue(LeftProperty, expZ * v.X * scale);
							control.SetValue(TopProperty, expZ * v.Y * scale);
							var size = value.Sender.Size;
							control.SetValue(WidthProperty, expZ * size.X * scale);
							control.SetValue(HeightProperty, expZ * size.Y * scale);
						})),
				
					newItem.WhenPropertyChanged(static p => p.Size)
						.Subscribe(new AnonymousObserver<PropertyValue<PhysicsObject, Vector3>>(value => {
							var expZ = Math.Exp(value.Sender.Position.Z);
							var size = value.Sender.Size;
							control.SetValue(WidthProperty, expZ * size.X * scale);
							control.SetValue(HeightProperty, expZ * size.Y * scale);
						}))
				};

				Children.Add(control);
			}
		}

		if (e.OldItems != null) {
			foreach (PhysicsObject oldItem in e.OldItems) {
				if (subscribes.Remove(oldItem, out var disposables)) {
					foreach (var disposable in disposables) {
						disposable.Dispose();
					}
				}

				for (var i = 0; i < Children.Count; i++) {
					if (Children[i].DataContext == oldItem) {
						Children.RemoveAt(i);
						break;
					}
				}
			}
		}

		ItemsChanged();
	}

	protected abstract void ItemsChanged();
}