using System;

public interface IObservable<T>
{
    event Action<T> OnChange;
    T Value { get; }
}