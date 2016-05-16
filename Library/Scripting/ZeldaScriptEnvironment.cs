
namespace Zelda.Scripting
{
    using System.Reflection;
    using Atom.Scripting;
    using Zelda.Profiles;

    public sealed class ZeldaScriptEnvironment : ScriptingEnvironment
    {
        public ZeldaScriptEnvironment( IZeldaServiceProvider serviceProvider )
        {
            this.helper = new DSLHelper( serviceProvider );

            LoadDSL();
        }

        private void LoadDSL()
        {
            var assembly = Assembly.GetExecutingAssembly();

            using( var stream = assembly.GetManifestResourceStream( "Zelda.Scripting.DSL.rb" ) )
            {
                this.Execute( stream );
            }

            this.SetGlobal( "dsl", this.helper );
        }

        /// <summary>
        /// Executes one script to 'warm boot' the system.
        /// This reduces the lag that sometimes occurres when executing the first script.
        /// </summary>
        private void WarmBoot()
        {
            if( hasWarmBooted || this.helper.Profile == null )
                return;

            var script = new ZeldaScript();
            script.Code = @"once_on fairy, 'warm-boot' do
                            end";

            script.Compile( this );
            script.Execute();
            hasWarmBooted = true;
        }

        public void OnProfileChanged( GameProfile profile )
        {
            this.SetGlobal( "profile", profile );
            this.SetGlobal( "keys", profile.KeySettings );

            this.SetGlobal( "world_status", profile.WorldStatus );
            this.SetGlobal( "player", profile.Player );
            this.SetGlobal( "fairy", profile.Player.Fairy );

            this.helper.Profile = profile;
        }

        public void OnSceneChanged( ZeldaScene scene )
        {
            this.SetGlobal( "scene", scene );
            this.SetGlobal( "scene_status", scene.Status );

            this.helper.Scene = scene;
            WarmBoot();
        }

        private bool hasWarmBooted;
        private readonly DSLHelper helper;
    }
}
