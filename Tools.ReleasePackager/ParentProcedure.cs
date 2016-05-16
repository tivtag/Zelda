// <copyright file="ParentProcedure.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Tools.ReleasePackager.ParentProcedure class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Tools.ReleasePackager
{
    using System;

    /// <summary>
    /// Defines an IProcedure that might run other IProcedures.
    /// </summary>
    public abstract class ParentProcedure : IProcedure
    {
        /// <summary>
        /// Runs this ParentProcedure.
        /// </summary>
        /// <returns>
        /// true if this IProcedure has sucessfully run;
        /// otherwise false.
        /// </returns>
        public abstract bool Run();
        
        /// <summary>
        /// Runs the specified IProcedure.
        /// </summary>
        /// <param name="procedure">
        /// The IProcedure to run.
        /// </param>
        /// <returns>
        /// true if the IProcedure has sucessfully run;
        /// otherwise false.
        /// </returns>
        protected bool RunProcedure( IProcedure procedure )
        {
            return this.RunProcedure( procedure, null );
        }

        /// <summary>
        /// Runs the specified IProcedure.
        /// </summary>
        /// <param name="procedure">
        /// The IProcedure to run.
        /// </param>
        /// <param name="description">
        /// The description that should be displayed for the procedure.
        /// </param>
        /// <returns>
        /// true if the IProcedure has sucessfully run;
        /// otherwise false.
        /// </returns>
        protected bool RunProcedure( IProcedure procedure, string description )
        {
            try
            {
                this.BeforeProcedure( description );
                
                bool result = procedure.Run();
                this.AfterProcedure( description );

                return result;
            }
            catch( Exception exc )
            {
                HandleProcedureException( procedure, description, exc );
                return false;
            }
        }

        /// <summary>
        /// Called before a sub IProcedure is run.
        /// </summary>
        /// <param name="description">
        /// The description that should be displayed for the procedure.
        /// </param>
        private void BeforeProcedure( string description )
        {
            if( description != null )
            {
                Console.Write( description );
            }
        }

        /// <summary>
        /// Called after a sub IProcedure has run.
        /// </summary>
        /// <param name="description">
        /// The description that should be displayed for the procedure.
        /// </param>
        private void AfterProcedure( string description )
        {
            if( description != null )
            {
                Console.WriteLine( ".. done" );
            }
        }

        /// <summary>
        /// Handles an exception that occurred while running a sub IProcedure.
        /// </summary>
        /// <param name="procedure">
        /// The IProcedure that has ran.
        /// </param>
        /// <param name="description">
        /// The description of the sub IProcedure.
        /// </param>
        /// <param name="exc">
        /// The exeception that has occurred.
        /// </param>
        private void HandleProcedureException( IProcedure procedure, string description, Exception exc )
        {
            Console.WriteLine( "====================" );
            Console.WriteLine( procedure.GetType().Name + " (" + description ?? string.Empty + ")" );
            Console.WriteLine( "An unexpected exception has occurred." );
            Console.WriteLine( "====================\n" );
            Console.WriteLine( exc.ToString() );
            Console.WriteLine();
        }
    }
}
