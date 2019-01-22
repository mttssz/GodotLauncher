using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GodotLauncher.Classes;

namespace GodotLauncher.ViewModels
{
    public class ConfigureViewModel : ViewModelBase
    {
        #region Properties

        #region GodotInstallLocation

        public const string GodotInstallLocationPropertyName = "GodotInstallLocation";
        private string m_GodotInstallLocation = "teszt";
        public string GodotInstallLocation
        {
            get { return m_GodotInstallLocation; }
            set
            {
                if ( m_GodotInstallLocation != null && m_GodotInstallLocation != value )
                    m_GodotInstallLocation = value;
            }
        }

        #endregion

        #region OnGodotLaunchSelectedIndex

        private int m_OnGodotLaunchSelectedIndex = 0;
        public int OnGodotLaunchSelectedIndex
        {
            get { return m_OnGodotLaunchSelectedIndex; }
            set
            {
                if ( m_OnGodotLaunchSelectedIndex != value )
                    SetProperty( ref m_OnGodotLaunchSelectedIndex, value );
            }
        }

        #endregion

        #region IsProxyEnabled

        private bool m_IsProxyEnabled = false;
        public bool IsProxyEnabled
        {
            get { return m_IsProxyEnabled; }
            set
            {
                if ( m_IsProxyEnabled != value )
                    SetProperty( ref m_IsProxyEnabled, value );
            }
        }

        #endregion

        #region ProxyServer

        private string m_ProxyServer = String.Empty;
        public string ProxyServer
        {
            get { return m_ProxyServer; }
            set
            {
                if ( m_ProxyServer != value )
                    SetProperty( ref m_ProxyServer, value );
            }
        }

        #endregion

        #region ProxyPort

        private int m_ProxyPort = 0;
        public int ProxyPort
        {
            get { return m_ProxyPort; }
            set
            {
                if ( m_ProxyPort != value )
                    SetProperty( ref m_ProxyPort, value );
            }
        }

        #endregion

        #endregion

        #region Commands

        private DelegateCommand m_SelectGodotInstallLocation = null;

        public DelegateCommand SelectGodotInstallLocation
        {
            get
            {
                return m_SelectGodotInstallLocation ?? (m_SelectGodotInstallLocation = new DelegateCommand( _SelectGodotInstallLocationCommandExecute, x => true ));
            }
        }

        private void _SelectGodotInstallLocationCommandExecute()
        {

        }

        #endregion
    }
}
