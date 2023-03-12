using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Reactive;
using static StartEx.PhysicsEngine.PhysicsObject;

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

	public static readonly DirectProperty<PhysicsPanel, PhysicsObject?> AttachedPhysicsObjectProperty =
		AvaloniaProperty.RegisterDirect<PhysicsPanel, PhysicsObject?>(nameof(ScaleProperty),
			x => x.attachedPhysicsObject, (x, v) => x.AttachedPhysicsObject = v);

	/// <summary>
	/// 附着到的物理对象
	/// </summary>
	public PhysicsObject? AttachedPhysicsObject {
		get => attachedPhysicsObject;
		set => SetAndRaise(AttachedPhysicsObjectProperty, ref attachedPhysicsObject, value);
	}

	private PhysicsObject? attachedPhysicsObject;

	protected PhysicsPanel() {
		var items = new ObservableCollection<PhysicsObject>();
		items.CollectionChanged += CollectionChanged;
		this.items = items;
	}

	private IDisposable? attachedPhysicsObjectSubscribe;

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
		} else if (change.Property == AttachedPhysicsObjectProperty) {
			attachedPhysicsObjectSubscribe?.Dispose();

			if (change.NewValue is PhysicsObject newObj) {
				attachedPhysicsObjectSubscribe = newObj.SubscribePositionChanging(new AnonymousObserver<Vector3ChangingArgs>(args => {
					if (items == null) {
						return;
					}

					var moveVec = args.NewValue - args.OldValue;  // 计算出运动的矢量
					foreach (PhysicsObject item in items) {
						item.Position -= moveVec;  // 加到子物体上
					}
				}));
			}
		}
	}

	private readonly Dictionary<PhysicsObject, IDisposable[]> childSubscribes = new();

	private void CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
		if (e.NewItems != null) {
			foreach (PhysicsObject newItem in e.NewItems) {
				var control = DataTemplates.FirstOrDefault(t => t.Match(newItem))?.Build(newItem) ?? new Border();
				control.DataContext = newItem;

				if (childSubscribes.TryGetValue(newItem, out var disposables)) {
					foreach (var disposable in disposables) {
						disposable.Dispose();
					}
				}

				childSubscribes[newItem] = new[] {
					newItem.SubscribePositionChanging(new AnonymousObserver<Vector3ChangingArgs>(args => {
						var v = args.NewValue;
						var expZ = Math.Exp(v.Z);
						control.SetValue(LeftProperty, expZ * v.X * scale);
						control.SetValue(TopProperty, expZ * v.Y * scale);
						var size = args.Sender.Size;
						control.SetValue(WidthProperty, expZ * size.X * scale);
						control.SetValue(HeightProperty, expZ * size.Y * scale);
					})),

					newItem.SubscribeSizeChanging(new AnonymousObserver<Vector3ChangingArgs>(args => {
						var expZ = Math.Exp(args.Sender.Position.Z);
						var size = args.NewValue;
						control.SetValue(WidthProperty, expZ * size.X * scale);
						control.SetValue(HeightProperty, expZ * size.Y * scale);
					}))
				};

				Children.Add(control);
			}
		}

		if (e.OldItems != null) {
			foreach (PhysicsObject oldItem in e.OldItems) {
				if (childSubscribes.Remove(oldItem, out var disposables)) {
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