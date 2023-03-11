using System.Collections.Generic;
using System.Collections.Specialized;

namespace StartEx.Core.Interfaces; 

public interface IObservableCollection<T> : INotifyCollectionChanged, IList<T> { }