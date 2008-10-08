using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;


namespace Gallio.Ambience.Server
{
    [RunInstaller(true)]
    public partial class AmbienceServiceInstaller : Installer
    {
        public AmbienceServiceInstaller()
        {
            InitializeComponent();
        }
    }
}
