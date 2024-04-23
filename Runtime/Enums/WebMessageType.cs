namespace ReupVirtualTwin.enums
{
    public class WebMessageType
    {
        public const string showMaterialsOptions = "showMaterialsOptions";
        public const string hideMaterialsOptions = "hideMaterialsOptions";

        public const string setEditMode = "[Edit Mode] Set Edit Mode";
        public const string setEditModeSuccess = "[Edit Mode] Set Edit Mode Success";

        public const string setSelectedObjects = "[Selected Objects] Set Selected Objects Success";

        public const string activatePositionTransform = "[Transform Objects] Activate Position Transform Mode";
        public const string activatePositionTransformSuccess = "[Transform Objects] Activate Position Transform Mode Success";
        public const string activateRotationTransform = "[Transform Objects] Activate Rotation Transform Mode";
        public const string activateRotationTransformSuccess = "[Transform Objects] Activate Rotation Transform Mode Success";
        public const string deactivateTransformMode = "[Transform Objects] Deactivate Transform Mode";
        public const string deactivateTransformModeSuccess = "[Transform Objects] Deactivate Transform Mode Success";

        public const string deleteObjects = "[Delete Objects] Delete Objects";
        public const string deleteObjectsSuccess = "[Delete Objects] Delete Objects Success";

        public const string loadObject = "[Load Objects] Load Object";
        public const string loadObjectSuccess = "[Load Objects] Load Object Success";
        public const string loadObjectProcessUpdate = "[Load Objects] Load Object Process Update";

        public const string changeObjectColor = "[Change Color] Change Object Color";
        public const string changeObjectColorSuccess = "[Change Color] Change Object Color Success";

        public const string error = "[Error] Engine Error";
        public const string startupMessage = "[Startup Message] Startup Message";
    }
}
