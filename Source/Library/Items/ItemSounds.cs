// <copyright file="ItemSounds.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the static Zelda.Items.ItemSounds class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Items
{
    using Atom.Fmod;
    using Atom.Math;

    /// <summary>
    /// Provides access to various item specific sounds.
    /// </summary>
    public static class ItemSounds
    {
        /// <summary>
        /// Plays the given Sound at the given volumne.
        /// </summary>
        /// <param name="sound">
        /// The Sound to play. Can be null.
        /// </param>
        /// <param name="volumne">
        /// The volumne of the sound; a value between 0 and 1.
        /// </param>
        private static void PlaySound( Sound sound, float volumne )
        {
            if( sound != null )
            {
                Channel channel = sound.Play( true );

                volumne += rand.RandomRange( -0.15f, 0.0f );

                channel.Volume = MathUtilities.Clamp( volumne, 0.0f, 1.0f );
                channel.Unpause();
            }
        }

        /// <summary>
        /// Randomly plays either pick-up or put-down sound of the given item.
        /// </summary>
        /// <param name="item">
        /// The item that is about to be picked-up/dropped down.
        /// </param>
        /// <param name="volumneMultiplicatorRange">
        /// The value the sound volume is multiplied with.
        /// </param>
        public static void PlayRandomPickUpOrDown( Item item, FloatRange volumneMultiplicatorRange )
        {
            float volumneMultiplicator = volumneMultiplicatorRange.GetRandomValue( rand );
            
            if( rand.RandomBoolean )
            {
                PlayPutDown( item, volumneMultiplicator );
            }
            else
            {
                PlayPickUp( item, volumneMultiplicator );
            }
        }

        /// <summary>
        /// Plays the sound associated with picking up
        /// the given Item.
        /// </summary>
        /// <param name="item">
        /// The item that is about to be picked-up.
        /// </param>
        /// <param name="volumneMultiplicator">
        /// The value the sound volume is multiplied with.
        /// </param>
        public static void PlayPickUp( Item item, float volumneMultiplicator = 1.0f )
        {
            float volumne;
            Sound sound = GetPickUp( item, out volumne );
            PlaySound( sound, volumne * volumneMultiplicator );
        }

        /// <summary>
        /// Gets the sound associated with picking up
        /// the given Item.
        /// </summary>
        /// <param name="item">
        /// The item that is about to be picked-up.
        /// </param>
        /// <param name="volumne">
        /// Will contain the volumne setting associated
        /// with the sound.
        /// </param>
        /// <returns>
        /// The associated Sound; might be null.
        /// </returns>
        private static Sound GetPickUp( Item item, out float volumne )
        {
            volumne = 1.0f;
            if( item.SoundOnPickup != null )
            {
                volumne = item.SoundOnPickupVolume.GetRandomValue( rand );
                return item.SoundOnPickup;
            }

            switch( item.ItemType )
            {
                case ItemType.Equipment:
                case ItemType.AffixedEquipment:
                    return GetPickUp( (Equipment)item, out volumne );

                case ItemType.Gem:
                    volumne = 0.4f;
                    return pickUpGem;

                default:
                    return GetPickUp( item.SpecialType, item, out volumne );
            }
        }

        /// <summary>
        /// Gets the sound associated with picking up
        /// the given Equipment.
        /// </summary>
        /// <param name="equipment">
        /// The Equipment that is about to be picked-up.
        /// </param>
        /// <param name="volumne">
        /// Will contain the volumne setting associated
        /// with the sound.
        /// </param>
        /// <returns>
        /// The associated Sound; might be null.
        /// </returns>
        private static Sound GetPickUp( Equipment equipment, out float volumne )
        {
            volumne = 1.0f;

            switch( equipment.Slot )
            {
                case EquipmentSlot.Necklace:
                case EquipmentSlot.Ring:
                    return pickUpRing;

                default:
                    return GetPickUp( equipment.SpecialType, equipment, out volumne );
            }
        }

        /// <summary>
        /// Gets the sound associated with picking up
        /// the given Item; taking into account the given SpecialItemType.
        /// </summary>
        /// <param name="specialType">
        /// The special type to take into account.
        /// </param>
        /// <param name="item">
        /// The Item that is about to be picked-up.
        /// </param>
        /// <param name="volumne">
        /// Will contain the volumne setting associated
        /// with the sound.
        /// </param>
        /// <returns>
        /// The associated Sound; might be null.
        /// </returns>
        private static Sound GetPickUp( SpecialItemType specialType, Item item, out float volumne )
        {
            volumne = 1.0f;

            switch( specialType )
            {
                case SpecialItemType.Cloth:
                    volumne = 0.55f;
                    return pickUpClothLeather;

                case SpecialItemType.Leather:
                    volumne = 0.65f;
                    return pickUpClothLeather;

                case SpecialItemType.Chains:
                    volumne = 0.7f;
                    return pickUpChains;

                case SpecialItemType.ChainsHeavy:
                    volumne = 0.6f;
                    return pickUpChainsHeavy;

                case SpecialItemType.Metal:
                    volumne = 0.65f;
                    return pickUpMetal;

                case SpecialItemType.MetalLight:
                    volumne = 0.4f;
                    return pickUpMetalLight;

                case SpecialItemType.MetalHeavy:
                    volumne = 0.4f;
                    return pickUpMetalHeavy;

                case SpecialItemType.Magical:
                    volumne = 0.7f;
                    return pickUpMagical;

                case SpecialItemType.Wood:
                    volumne = 0.9f;
                    return pickUpWood;

                case SpecialItemType.Ore:
                    volumne = 0.4f;
                    return pickUpOre;

                case SpecialItemType.Gem:
                    volumne = 0.4f;
                    return pickUpGem;

                case SpecialItemType.Food:
                    volumne = 0.5f;
                    return pickUpFood;

                case SpecialItemType.Key:
                    volumne = 0.3f;
                    return pickUpKey;

                case SpecialItemType.Rock:
                    volumne = 0.8f;
                    return pickUpOre;

                case SpecialItemType.Liquid:
                    volumne = 0.7f;
                    return pickUpLiquid;

                case SpecialItemType.LiquidHeavy:
                    return pickUpLiquid;

                case SpecialItemType.Organic:
                case SpecialItemType.Herb:
                    volumne = 0.7f;
                    return pickUpHerb;

                case SpecialItemType.Parchment:
                    volumne = 0.6f;
                    return pickUpParchment;

                case SpecialItemType.Skull:
                    volumne = 0.5f;
                    return pickUpSkull;

                case SpecialItemType.Bone:
                    volumne = 0.5f;
                    return pickUpBone;

                case SpecialItemType.Quiver:
                    volumne = 0.5f;
                    return pickUpQuiver;

                case SpecialItemType.Jewelry:
                    return pickUpRing;

                default:
                    volumne = item.SoundOnPickupVolume.GetRandomValue( rand );
                    return item.SoundOnPickup;
            }
        }

        /// <summary>
        /// Plays the sound associated with putting down
        /// the given Item.
        /// </summary>
        /// <param name="item">
        /// The item that is about to be put-down.
        /// </param>
        /// <param name="volumneMultiplicator">
        /// The value the sound volume is multiplied with.
        /// </param>
        public static void PlayPutDown( Item item, float volumneMultiplicator = 1.0f )
        {
            float volumne;
            Sound sound = GetPutDown( item, out volumne );
            PlaySound( sound, volumne * volumneMultiplicator );
        }

        /// <summary>
        /// Gets the sound associated with putting down
        /// the given Item.
        /// </summary>
        /// <param name="item">
        /// The item that is about to be put down.
        /// </param>
        /// <param name="volumne">
        /// Will contain the volumne setting associated
        /// with the sound.
        /// </param>
        /// <returns>
        /// The associated Sound; might be null.
        /// </returns>
        private static Sound GetPutDown( Item item, out float volumne )
        {
            volumne = 1.0f;

            switch( item.ItemType )
            {
                case ItemType.Equipment:
                case ItemType.AffixedEquipment:
                    return GetPutDown( (Equipment)item, out volumne );

                case ItemType.Gem:
                    volumne = 0.7f;
                    return putDownGem;

                default:
                    return GetPutDown( item.SpecialType, item, out volumne );
            }
        }

        /// <summary>
        /// Gets the sound associated with putting down
        /// the given Equipment.
        /// </summary>
        /// <param name="equipment">
        /// The Equipment that is about to be put down.
        /// </param>
        /// <param name="volumne">
        /// Will contain the volumne setting associated
        /// with the sound.
        /// </param>
        /// <returns>
        /// The associated Sound; might be null.
        /// </returns>
        private static Sound GetPutDown( Equipment equipment, out float volumne )
        {
            volumne = 1.0f;

            switch( equipment.Slot )
            {
                case EquipmentSlot.Ring:
                case EquipmentSlot.Necklace:
                    return putDownRing;

                default:
                    return GetPutDown( equipment.SpecialType, equipment, out volumne );
            }
        }

        /// <summary>
        /// Gets the sound associated with putting down
        /// the given Item; taking into account the given SpecialItemType.
        /// </summary>
        /// <param name="specialType">
        /// The special type to take into account.
        /// </param>
        /// <param name="item">
        /// The Item that is about to be put down.
        /// </param>
        /// <param name="volumne">
        /// Will contain the volumne setting associated
        /// with the sound.
        /// </param>
        /// <returns>
        /// The associated Sound; might be null.
        /// </returns>
        private static Sound GetPutDown( SpecialItemType specialType, Item item, out float volumne )
        {
            volumne = 1.0f;

            switch( specialType )
            {
                case SpecialItemType.Cloth:
                    volumne = 0.5f;
                    return putDownClothLeather;

                case SpecialItemType.Leather:
                    volumne = 0.6f;
                    return putDownClothLeather;

                case SpecialItemType.Chains:
                    volumne = 0.7f;
                    return putDownChains;

                case SpecialItemType.ChainsHeavy:
                    volumne = 0.6f;
                    return putDownChainsHeavy;

                case SpecialItemType.Metal:
                    volumne = 0.65f;
                    return putDownMetal;

                case SpecialItemType.MetalLight:
                    volumne = 0.6f;
                    return putDownMetalLight;

                case SpecialItemType.MetalHeavy:
                    volumne = 0.4f;
                    return putDownMetalHeavy;

                case SpecialItemType.Magical:
                    volumne = 0.7f;
                    return putDownMagical;

                case SpecialItemType.Ore:
                    volumne = 0.45f;
                    return putDownOre;

                case SpecialItemType.Gem:
                    volumne = 0.6f;
                    return putDownGem;

                case SpecialItemType.Food:
                    volumne = 0.6f;
                    return putDownFood;

                case SpecialItemType.Key:
                    volumne = 0.4f;
                    return putDownKey;

                case SpecialItemType.Rock:
                    volumne = 0.8f;
                    return putDownOre;

                case SpecialItemType.Wood:
                    volumne = 0.9f;
                    return putDownWood;

                case SpecialItemType.Liquid:
                    volumne = 0.7f;
                    return putDownLiquid;

                case SpecialItemType.LiquidHeavy:
                    return putDownLiquid;

                case SpecialItemType.Organic:
                case SpecialItemType.Herb:
                    volumne = 0.7f;
                    return putDownHerb;

                case SpecialItemType.Parchment:
                    volumne = 0.5f;
                    return putDownParchment;

                case SpecialItemType.Skull:
                    volumne = 0.6f;
                    return putDownSkull;

                case SpecialItemType.Bone:
                    volumne = 0.6f;
                    return putDownBone;

                case SpecialItemType.Quiver:
                    volumne = 0.6f;
                    return putDownQuiver;

                case SpecialItemType.Jewelry:
                    return putDownRing;

                default:
                    volumne = item.SoundOnPickupVolume.GetRandomValue( rand );
                    return item.SoundOnPickup;
            }
        }

        /// <summary>
        /// Loads the ItemSounds. 
        /// </summary>
        /// <param name="audioSystem">
        /// The AudioSystem to use.
        /// </param>
        /// <param name="rand">
        /// A random number generator.
        /// </param>
        public static void Load( AudioSystem audioSystem, Atom.Math.RandMT rand )
        {
            ItemSounds.rand = rand;

            pickUpRing = LoadPickUp( "Ring", audioSystem );
            putDownRing = LoadPutDown( "Ring", audioSystem );

            pickUpGem = LoadPickUp( "Gem", audioSystem );
            putDownGem = LoadPutDown( "Gem", audioSystem );

            pickUpLiquid = LoadPickUp( "Liquid", audioSystem );
            putDownLiquid = LoadPutDown( "Liquid", audioSystem );

            pickUpOre = LoadPickUp( "Ore", audioSystem );
            putDownOre = LoadPutDown( "Ore", audioSystem );

            pickUpHerb = LoadPickUp( "Herb", audioSystem );
            putDownHerb = LoadPutDown( "Herb", audioSystem );

            pickUpFood = LoadPickUp( "Food", audioSystem );
            putDownFood = LoadPutDown( "Food", audioSystem );

            pickUpKey = LoadPickUp( "Key", audioSystem );
            putDownKey = LoadPutDown( "Key", audioSystem );

            pickUpParchment = LoadPickUp( "Parchment", audioSystem );
            putDownParchment = LoadPutDown( "Parchment", audioSystem );

            pickUpClothLeather = LoadPickUp( "ClothLeather", audioSystem );
            putDownClothLeather = LoadPutDown( "ClothLeather", audioSystem );

            pickUpChains = LoadPickUp( "Chains", audioSystem );
            putDownChains = LoadPutDown( "Chains", audioSystem );

            pickUpChainsHeavy = LoadPickUp( "ChainsHeavy", audioSystem );
            putDownChainsHeavy = LoadPutDown( "ChainsHeavy", audioSystem );

            pickUpMetal = LoadPickUp( "Metal", audioSystem );
            putDownMetal = LoadPutDown( "Metal", audioSystem );

            pickUpMetalLight = LoadSample( "PickUpPutDown_MetalLight.wav", audioSystem );
            putDownMetalLight = pickUpMetalLight;

            pickUpMetalHeavy = LoadPickUp( "MetalHeavy", audioSystem );
            putDownMetalHeavy = LoadPutDown( "MetalHeavy", audioSystem );

            pickUpWood = LoadPickUp( "Wood", audioSystem );
            putDownWood = LoadPutDown( "Wood", audioSystem );

            pickUpMagical = LoadPickUp( "Magical", audioSystem );
            putDownMagical = LoadPutDown( "Magical", audioSystem );

            pickUpSkull = LoadSample( "PickUpPutDown_Skull.wav", audioSystem );
            putDownSkull = pickUpSkull;

            pickUpBone = LoadSample( "PickUpPutDown_Bone.wav", audioSystem );
            putDownBone = pickUpBone;

            pickUpQuiver = LoadSample( "PickUpPutDown_Quiver.wav", audioSystem );
            putDownQuiver = pickUpQuiver;
        }

        /// <summary>
        /// Loads the sound for picking up an item.
        /// </summary>
        /// <param name="name">
        /// The base name of the sound to load.
        /// </param>
        /// <param name="audioSystem">
        /// The AudioSystem to use.
        /// </param>
        /// <returns>
        /// The loaded sound.
        /// </returns>
        private static Sound LoadPickUp( string name, AudioSystem audioSystem )
        {
            return LoadSample( "PickUp_" + name + ".wav", audioSystem );
        }

        /// <summary>
        /// Loads the sound for putting down an item.
        /// </summary>
        /// <param name="name">
        /// The base name of the sound to load.
        /// </param>
        /// <param name="audioSystem">
        /// The AudioSystem to use.
        /// </param>
        /// <returns>
        /// The loaded sound.
        /// </returns>
        private static Sound LoadPutDown( string name, AudioSystem audioSystem )
        {
            return LoadSample( "PutDown_" + name + ".wav", audioSystem );
        }

        /// <summary>
        /// Loads the sound sample with the given name.
        /// </summary>
        /// <param name="fullName">
        /// The full name of the sound to load.
        /// </param>
        /// <param name="audioSystem">
        /// The AudioSystem to use.
        /// </param>
        /// <returns>
        /// The loaded sound.
        /// </returns>
        private static Sound LoadSample( string fullName, AudioSystem audioSystem )
        {
            Sound sample = audioSystem.GetSample( fullName );
            if( sample == null )
            {
                return null;
            }

            sample.LoadAsSample( false );
            return sample;
        }

        /// <summary>
        /// The sounds for picking-up or putting-down a RING.
        /// </summary>
        private static Sound pickUpRing, putDownRing;

        /// <summary>
        /// The sounds for picking-up or putting-down a GEM.
        /// </summary>
        private static Sound pickUpGem, putDownGem;

        /// <summary>
        /// The sounds for picking-up or putting-down a LIQUID.
        /// </summary>
        private static Sound pickUpLiquid, putDownLiquid;

        /// <summary>
        /// The sounds for picking-up or putting-down a ORE or ROCK.
        /// </summary>
        private static Sound pickUpOre, putDownOre;

        /// <summary>
        /// The sounds for picking-up or putting-down an item made of PAPER.
        /// </summary>
        private static Sound pickUpParchment, putDownParchment;

        /// <summary>
        /// The sounds for picking-up or putting-down a HERB.
        /// </summary>
        private static Sound pickUpHerb, putDownHerb;

        /// <summary>
        /// The sounds for picking-up or putting-down a METAL item.
        /// </summary>
        private static Sound pickUpMetal, putDownMetal;

        /// <summary>
        /// The sounds for picking-up or putting-down a LIGHT-WEIGHT METAL item.
        /// </summary>
        private static Sound pickUpMetalLight, putDownMetalLight;

        /// <summary>
        /// The sounds for picking-up or putting-down a WOOD item.
        /// </summary>
        private static Sound pickUpWood, putDownWood;

        /// <summary>
        /// The sounds for picking-up or putting-down a HEAVY METAL item.
        /// </summary>
        private static Sound pickUpMetalHeavy, putDownMetalHeavy;

        /// <summary>
        /// The sounds for picking-up or putting-down a HEAVY CHAINS item.
        /// </summary>
        private static Sound pickUpChainsHeavy, putDownChainsHeavy;

        /// <summary>
        /// The sounds for picking-up or putting-down a CHAINS item.
        /// </summary>
        private static Sound pickUpChains, putDownChains;

        /// <summary>
        /// The sounds for picking-up or putting-down a CLOTH or LEATHER.
        /// </summary>
        private static Sound pickUpClothLeather, putDownClothLeather;

        /// <summary>
        /// The sounds for picking-up or putting-down a MAGICAL item.
        /// </summary>
        private static Sound pickUpMagical, putDownMagical;

        /// <summary>
        /// The sounds for picking-up or putting-down a SKULL.
        /// </summary>
        private static Sound pickUpSkull, putDownSkull;

        /// <summary>
        /// The sounds for picking-up or putting-down a BONE.
        /// </summary>
        private static Sound pickUpBone, putDownBone;

        /// <summary>
        /// The sounds for picking-up or putting-down a QUIVER.
        /// </summary>
        private static Sound pickUpQuiver, putDownQuiver;

        /// <summary>
        /// The sounds for picking-up or putting-down a KEY.
        /// </summary>
        private static Sound pickUpKey, putDownKey;

        /// <summary>
        /// The sounds for picking-up or putting-down FOOD.
        /// </summary>
        private static Sound pickUpFood, putDownFood;

        /// <summary>
        /// A random number generator.
        /// </summary>
        private static Atom.Math.RandMT rand;
    }
}
