// <copyright file="ItemSocketRenderer.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.Items.ItemSocketRenderer class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.UI.Items
{
    using Atom.Math;
    using Atom.Xna;
    using Atom.Xna.Batches;
    using Zelda.Items;

    /// <summary>
    /// Responsible for rendering the sockets and gems of an EquipmentInstance.
    /// </summary>
    internal sealed class ItemSocketRenderer
    {
        /// <summary>
        /// Initializes a new instance of the ItemSocketRenderer class.
        /// </summary>
        /// <param name="spriteLoader">
        /// Provides a mechanism that allows the loading of Sprite assets.
        /// </param>
        public ItemSocketRenderer( ISpriteLoader spriteLoader )
        {
            this.spriteFireSocket       = spriteLoader.LoadSprite( "InventoryCell_Gem_Fire" );
            this.spriteWaterSocket      = spriteLoader.LoadSprite( "InventoryCell_Gem_Water" );
            this.spriteLightSocket      = spriteLoader.LoadSprite( "InventoryCell_Gem_Light" );
            this.spriteNatureSocket     = spriteLoader.LoadSprite( "InventoryCell_Gem_Nature" );
            this.spriteShadowSocket     = spriteLoader.LoadSprite( "InventoryCell_Gem_Shadow" );
            this.spritePristmaticSocket = spriteLoader.LoadSprite( "InventoryCell_Gem_Prismatic" );
        }

        /// <summary>
        /// Draws the Sockets of the given ItemInstance at the given position.
        /// </summary>
        /// <param name="itemInstance">
        /// The ItemInstance whose sockets should be drawn.
        /// </param>
        /// <param name="drawPosition">
        /// The position the ItemInstance was drawn.
        /// </param>
        /// <param name="depth">
        /// The depth to draw at.
        /// </param>
        /// <param name="batch">
        /// The XNA SpriteBatch object to use.
        /// </param>
        public void DrawSockets( ItemInstance itemInstance, Vector2 drawPosition, float depth, ISpriteBatch batch )
        {
            var equipmentInstance = itemInstance as EquipmentInstance;
            if( equipmentInstance == null )
                return;

            var socketProperties = equipmentInstance.SocketProperties;
            int socketCount = socketProperties.SocketCount;
            if( socketCount == 0 )
                return;

            float depthAbove = depth + 0.0025f;
            var item = itemInstance.Item;

            for( int index = 0; index < socketCount; ++index )
            {
                Socket socket = socketProperties.GetSocket( index );
                Vector2 socketPosition = drawPosition + GetSocketOffset( index, item );

                if( socket.IsEmpty )
                {
                    var sprite = this.GetSocketSprite( socket.Color );
                    sprite.Draw( socketPosition, depthAbove, batch );
                }
            }
        }

        /// <summary>
        /// Gets the socket position at the given index relative to the given Item.
        /// </summary>
        /// <param name="socketIndex">
        /// The index of the socket.
        /// </param>
        /// <param name="item">
        /// The Item to which the the socket position should be relative to.
        /// </param>
        /// <returns>
        /// The position.
        /// </returns>
        public static Vector2 GetSocketOffset( int socketIndex, Item item )
        {
            return new Vector2(
                (socketIndex % item.InventoryWidth) * UIConstants.CellSize,
                (socketIndex / item.InventoryWidth) * UIConstants.CellSize
            );
        }

        /// <summary>
        /// Gets the cell sprite associated with Gem of the given ElementalSchool.
        /// </summary>
        /// <param name="gemColor">
        /// The color of the Gem.
        /// </param>
        /// <returns>
        /// The requested Sprite.
        /// </returns>
        private Sprite GetSocketSprite( Zelda.Status.ElementalSchool gemColor )
        {
            switch( gemColor )
            {
                case Zelda.Status.ElementalSchool.All:
                    return this.spritePristmaticSocket;

                case Zelda.Status.ElementalSchool.Nature:
                    return this.spriteNatureSocket;

                case Zelda.Status.ElementalSchool.Water:
                    return this.spriteWaterSocket;

                case Zelda.Status.ElementalSchool.Shadow:
                    return this.spriteShadowSocket;

                case Zelda.Status.ElementalSchool.Light:
                    return this.spriteLightSocket;

                case Zelda.Status.ElementalSchool.Fire:
                default:
                    return this.spriteFireSocket;
            }
        }

        /// <summary>
        /// The sprites used to visualize the gem slots of an item.
        /// </summary>
        private readonly Sprite 
            spriteFireSocket,
            spriteWaterSocket,
            spriteLightSocket,
            spriteNatureSocket,
            spriteShadowSocket,
            spritePristmaticSocket;
    }
}
