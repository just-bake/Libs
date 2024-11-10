using System;
using System.Collections.Generic;

namespace Libs.justbake.MVC.Watchables
{
    public class Watchable<T>
    {
        public event EventHandler<ValueChangedEventArgs<T>>? ValueChanged;
        
        private T _watchedValue;
        
        public T WatchedValue
        {
            get => _watchedValue;
            set
            {
                if (EqualityComparer<T>.Default.Equals(_watchedValue, value)) return;
                
                T oldValue = _watchedValue;
                _watchedValue = value;
                OnValueChanged(new ValueChangedEventArgs<T>(oldValue, _watchedValue));
            }
        }
        
        public Watchable(T watchedValue)
        {
            _watchedValue = watchedValue;
        }
        
        protected virtual void OnValueChanged(ValueChangedEventArgs<T> e)
        {
            ValueChanged?.Invoke(this, e);
        }
        
        public static implicit operator T(Watchable<T> watchable) => watchable.WatchedValue;
        public static explicit operator Watchable<T>(T @value) => new Watchable<T>(@value);
    }
}