
namespace Zelda.Scripting.Design
{
    using System;
    using System.ComponentModel;
    using System.Drawing.Design;
    using Zelda.Design;

    public sealed class ScriptCodeEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle( ITypeDescriptorContext context )
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue( ITypeDescriptorContext context, IServiceProvider provider, object value )
        {
            var editor = DesignTime.GetService<IScriptEditor>();
            return editor.EditCode( (value as string) ?? string.Empty );
        }
    }
}
