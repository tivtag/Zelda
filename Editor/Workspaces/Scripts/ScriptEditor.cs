
namespace Zelda.Editor.Workspaces.Scripts
{
    using Zelda.Scripting.Design;
    using Zelda.Design;

    public sealed class ScriptEditor : IScriptEditor
    {
        public string EditCode( string initialCode )
        {
            var dialog = new ScriptEditorWindow() {
                Code = initialCode
            };

            if( dialog.ShowDialog() == true )
            {
                return dialog.Code;
            }

            return initialCode;
        }
    }
}
