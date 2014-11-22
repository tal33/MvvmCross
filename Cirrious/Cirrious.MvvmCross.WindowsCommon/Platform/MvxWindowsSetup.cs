// MvxStoreSetup.cs
// (c) Copyright Cirrious Ltd. http://www.cirrious.com
// MvvmCross is licensed using Microsoft Public License (Ms-PL)
// Contributions and inspirations noted in readme.md and license.txt
// 
// Project Lead - Stuart Lodge, @slodge, me@slodge.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Platform;
using Cirrious.CrossCore.Plugins;
using Cirrious.MvvmCross.Platform;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.MvvmCross.Views;
using Cirrious.MvvmCross.WindowsCommon.Views;
using Cirrious.MvvmCross.WindowsCommon.Views.Suspension;

namespace Cirrious.MvvmCross.WindowsCommon.Platform
{
    public interface IMvxWindowsFrame
    {
        Control UnderlyingControl { get; }
        CoreDispatcher Dispatcher { get; }
        object Content { get; }
        bool CanGoBack { get; }
        bool Navigate(Type viewType, object parameter);
        void GoBack();
        void ClearValue(DependencyProperty property);
        object GetValue(DependencyProperty property);
        void SetValue(DependencyProperty property, object value);
        void SetNavigationState(string state);
        string GetNavigationState();
    }



    public class MvxFrameAdapter : IMvxWindowsFrame
    {
        private readonly Frame _frame;

        public MvxFrameAdapter(Frame frame)
        {
            _frame = frame;
        }

        public Control UnderlyingControl { get { return _frame; } }

        public CoreDispatcher Dispatcher { get { return _frame.Dispatcher; } }
        public object Content { get { return _frame.Content; } }
        public bool CanGoBack { get { return _frame.CanGoBack; } }

        public bool Navigate(Type viewType, object parameter)
        {
            return _frame.Navigate(viewType, parameter);
        }

        public void GoBack()
        {
            _frame.GoBack();
        }

        public void ClearValue(DependencyProperty property)
        {
            _frame.ClearValue(property);
;        }


        public object GetValue(DependencyProperty property)
        {
            return _frame.GetValue(property);
        }

        public void SetValue(DependencyProperty property, object value)
        {
            _frame.SetValue(property, value);
        }

        public void SetNavigationState(string state)
        {
            _frame.SetNavigationState(state);
        }

        public string GetNavigationState()
        {
            return _frame.GetNavigationState();
        }
    }

    public abstract class MvxWindowsSetup
        : MvxSetup
    {
        private readonly IMvxWindowsFrame _rootFrame;

        protected MvxWindowsSetup(Frame rootFrame)
            : this(new MvxFrameAdapter(rootFrame))
        {
        }

        protected MvxWindowsSetup(IMvxWindowsFrame rootFrame)
        {
            _rootFrame = rootFrame;
        }

        protected override IMvxTrace CreateDebugTrace()
        {
            return new MvxDebugTrace();
        }

        protected override void InitializePlatformServices()
        {
            InitializeSuspensionManager();
            base.InitializePlatformServices();
        }

        protected virtual void InitializeSuspensionManager()
        {
            var suspensionManager = CreateSuspensionManager();
            Mvx.RegisterSingleton(suspensionManager);
        }

        protected virtual IMvxSuspensionManager CreateSuspensionManager()
        {
            return new MvxSuspensionManager();
        }

        protected override IMvxPluginManager CreatePluginManager()
        {
            return new MvxFilePluginManager(new List<string>() { ".WindowsCommon", ".WindowsPhoneStore", ".WindowsStore" });
        }

        protected sealed override IMvxViewsContainer CreateViewsContainer()
        {
            return CreateStoreViewsContainer();
        }

        protected virtual IMvxStoreViewsContainer CreateStoreViewsContainer()
        {
            return new MvxWindowsViewsContainer();
        }

        protected override IMvxViewDispatcher CreateViewDispatcher()
        {
            return CreateViewDispatcher(_rootFrame);
        }

        protected virtual IMvxWindowsViewPresenter CreateViewPresenter(IMvxWindowsFrame rootFrame)
        {
            return new MvxWindowsViewPresenter(rootFrame);
        }

        protected virtual MvxWindowsViewDispatcher CreateViewDispatcher(IMvxWindowsFrame rootFrame)
        {
            var presenter = CreateViewPresenter(_rootFrame);
            return new MvxWindowsViewDispatcher(presenter, rootFrame);
        }

        protected override IMvxNameMapping CreateViewToViewModelNaming()
        {
            return new MvxPostfixAwareViewToViewModelNameMapping("View", "Page");
        }
    }
}