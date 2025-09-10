using System;

namespace UI.Base
{
    public abstract class BaseUI<T, TViewModel, TView, TData> : BaseUI<TData>
        where TViewModel : BaseUIViewModel, new()
        where TView : UIView<TViewModel>
        where T : BaseUI<TData>, new()
        where TData : struct
    {
        protected TViewModel ViewModel { get; set; }
        
        protected TView View => (TView)BaseUIView;

        public static T Add(TData data)
        {
            return UI.Add<T, TData>(data);
        }
        
        public static void Remove(T ui)
        {
            UI.Remove(ui);
        }
        
        public static T Open(TData data, bool instant)
        {
            return UI.Open<T, TData>(data, instant);
        }

        public static void Close(T ui, bool instant = false)
        {
            UI.Close(ui, instant);
        }

        internal sealed override void OnConnectView()
        {
            ViewModel = ((TView)BaseUIView).ViewModel;
            OnConnect();
        }

        protected virtual void OnConnect()
        {
        }
    }
    
    public abstract class BaseUI<TData> : BaseUI where TData : struct
    {
        protected TData Data { get; private set; }

        internal void SetData(TData data)
        {
            Data = data;
            OnSetData();
        }

        protected virtual void OnSetData()
        {
        }
    }

    public abstract class BaseUI
    {
        public Action OnOpenAction;
        public Action OnCloseAction;
        public Action OnAddAction;
        public Action OnRemoveAction;
        
        internal BaseUIView BaseUIView { get; private set; }
        
        internal void ConnectView(BaseUIView baseUIView)
        {
            BaseUIView = baseUIView;
            OnConnectView();
        }

        internal abstract void OnConnectView();
        
        internal void Add()
        {
            Add(this);
            OnAddAction?.Invoke();
        }
        
        internal void Remove()
        {
            Remove(this);
            OnRemoveAction?.Invoke();
        }
        
        internal void Open(bool instant = false)
        {
            Open(this, instant);
            OnOpenAction?.Invoke();
        }
        
        internal void Close(bool instant = false)
        {
            Close(this, instant);
            OnCloseAction?.Invoke();
        }
        
        protected virtual void OnAdd()
        {
        }
        
        protected virtual void OnRemove()
        {
        }
        
        protected virtual void OnOpen()
        {
        }

        protected virtual void OnOpening()
        {
        }

        protected virtual void OnClose()
        {
        }

        protected virtual void OnClosing()
        {
        }
        
        private static void Add(BaseUI ui)
        {
            ui.OnAdd(); 
            ui.BaseUIView.OnAdd();
            ui.BaseUIView.SetActive(true);
            if (ui.BaseUIView.Raycaster != null)
            {
                ui.BaseUIView.Raycaster.enabled = true;
            }
        }
        
        private static void Remove(BaseUI ui)
        {
            ui.BaseUIView.SetActive(false);
            if (ui.BaseUIView.Raycaster != null)
            {
                ui.BaseUIView.Raycaster.enabled = false;
            }

            ui.OnRemove();
        }
        
        private static void Open(BaseUI ui, bool instant)
        {
            if (instant)
            {
                ui.OnOpen();
                ui.BaseUIView.SetActive(true);
            }

            if (ui.BaseUIView.Raycaster != null)
            {
                ui.BaseUIView.Raycaster.enabled = true;
            }

            ui.OnOpening();
        }
        
        private static void Close(BaseUI ui, bool instant)
        {
            if (instant)
            {
                ui.BaseUIView.SetActive(false);
                ui.OnClose();
            }

            if (ui.BaseUIView.Raycaster != null)
            {
                ui.BaseUIView.Raycaster.enabled = false;
            }

            ui.OnClosing();
        }
    }
}