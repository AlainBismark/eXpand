using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Xpo;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.Persistent.Base;

namespace Xpand.ExpressApp.ModelDifference.DictionaryStores {
    public abstract class XpoDictionaryDifferenceStore : ModelDifferenceStore {
        private readonly XPObjectSpace _objectSpace;

        protected XpoDictionaryDifferenceStore(XafApplication application) {
            Application = application;
            _objectSpace = (XPObjectSpace)application.CreateObjectSpace(typeof(ModelDifferenceObject));
        }


        public XafApplication Application { get; }

        public XPObjectSpace ObjectSpace => _objectSpace;

        public override string Name => DifferenceType.ToString();

        public abstract DifferenceType DifferenceType { get; }

        public override void SaveDifference(ModelApplicationBase model) {
            if (model != null) {
                ModelDifferenceObject modelDifferenceObject =
                    GetActiveDifferenceObject(model.Id) ??
                    GetNewDifferenceObject(_objectSpace)
                    .InitializeMembers(model.Id == "Application" ? Application.Title : model.Id, Application.Title, Application.GetType().FullName);
                if (!_objectSpace.IsNewObject(modelDifferenceObject))
                    _objectSpace.ReloadObject(modelDifferenceObject);
                OnDifferenceObjectSaving(modelDifferenceObject, model);
            }
        }

        protected internal abstract ModelDifferenceObject GetActiveDifferenceObject(string name);

        protected internal abstract ModelDifferenceObject GetNewDifferenceObject(IObjectSpace objectSpace);

        protected internal virtual void OnDifferenceObjectSaving(ModelDifferenceObject modelDifferenceObject, ModelApplicationBase model) {
            ObjectSpace.CommitChanges();
        }
    }
}