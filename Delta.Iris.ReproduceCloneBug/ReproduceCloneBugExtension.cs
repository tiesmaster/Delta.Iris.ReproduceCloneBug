using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

using log4net;

using ESRI.ArcGIS.Desktop.AddIns;

using AedSicad.UT.Gis.CoreRuntime;
using AedSicad.UT.Foundation;

namespace Delta.Iris.ReproduceCloneBug
{
    public class ReproduceCloneBugExtension : Extension, IUtilityApplicationShellHook
    {
        static readonly log4net.ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ReproduceCloneBugExtension() { }

        protected override void OnStartup()
        {
            UTRuntimeEnvironment.Instance.WorkspaceChanged += HookupToDatasource;
        }

        void HookupToDatasource(object sender, WorkspaceChangedEventArgs e)
        {
            var shell = GetShell();
            if (shell == null)
            {
                Log.Error("Failed to retrieve the UT shell.");
                return;
            }
            GetHooked(shell);
        }

        UtilityApplicationShell GetShell()
        {
            return UTRuntimeEnvironment.Instance.Shell;
        }


        public void GetHooked(UtilityApplicationShell shell)
        {
            shell.Datasource.AssetCreated += Datasource_AssetCreated;
        }

        void Datasource_AssetCreated(object sender, AssetEventArgs e)
        {
            var asset = e.Asset;
            Log.InfoFormat("Datasource_AssetCreated: {0}", FormatAsset(asset));
            asset.Cloned += asset_Cloned;
        }

        string FormatAsset(Asset asset)
        {
            return string.Format("{0}[OBJ_ID={1}]", asset.UtilityClass, asset.UTObjId);
        }

        void asset_Cloned(object sender, AssetCloneEventArgs e)
        {
            var originalAsset = e.Asset;
            var clonedAsset = e.ClonedAsset;
            Log.InfoFormat("asset_Cloned: original asset: {0}, cloned asset: {1}",
                FormatAsset(originalAsset), FormatAsset(clonedAsset));
        }

        public string ImplementationInfo
        {
            get { return "Delta.Iris.ReproduceCloneBug"; }
        }
    }
}