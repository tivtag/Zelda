// <copyright file="ZeldaWaypointMap.ReaderWriter.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Waypoints.ZeldaWaypointMap.ReaderWriter class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Waypoints
{
    using Atom;
    using Atom.Math;
    using Atom.Storage;
    using Atom.Waypoints;

    /// <summary>
    /// Contains the <see cref="ZeldaWaypointMap.ReaderWriter"/> class.
    /// </summary>
    public partial class ZeldaWaypointMap
    {
        /// <summary>
        /// Defines the IObjectReaderWriter for the ZeldaWaypointMap class.
        /// </summary>
        public sealed class ReaderWriter : BaseObjectReaderWriter<ZeldaWaypointMap>
        {
            /// <summary>
            /// Serializes the given object using the given ISerializationContext.
            /// </summary>
            /// <param name="object">
            /// The object to serialize.
            /// </param>
            /// <param name="context">
            /// The context that provides everything required for the serialization process.
            /// </param>
            public override void Serialize( ZeldaWaypointMap @object, ISerializationContext context )
            {
                const int CurrentVersion = 2;
                context.Write( CurrentVersion );

                SerializeWaypoints( @object, context );
                SerializeSegments( @object, context );
                SerializePaths( @object, context );
            }

            /// <summary>
            /// Serializes the Waypoints of the given ZeldaWaypointMap using the given ISerializationContext.
            /// </summary>
            /// <param name="waypointMap">
            /// The object to serialize.
            /// </param>
            /// <param name="context">
            /// The context that provides everything required for the serialization process.
            /// </param>
            private static void SerializeWaypoints( ZeldaWaypointMap waypointMap, ISerializationContext context )
            {
                const int CurrentVersion = 1;
                context.Write( CurrentVersion );
                context.Write( waypointMap.WaypointCount );

                for( int i = 0; i < waypointMap.WaypointCount; ++i )
                {
                    var waypoint = (ZeldaWaypoint)waypointMap.GetWaypointAt( i );

                    context.Write( waypoint.Name );
                    context.Write( waypoint.FloorNumber );
                    context.Write( waypoint.Position );
                }
            }

            /// <summary>
            /// Serializes the PathSegments of the given ZeldaWaypointMap using the given ISerializationContext.
            /// </summary>
            /// <param name="waypointMap">
            /// The object to serialize.
            /// </param>
            /// <param name="context">
            /// The context that provides everything required for the serialization process.
            /// </param>
            private static void SerializeSegments( ZeldaWaypointMap waypointMap, ISerializationContext context )
            {
                const int CurrentVersion = 1;
                context.Write( CurrentVersion );
                context.Write( waypointMap.PathSegmentCount );

                for( int i = 0; i < waypointMap.PathSegmentCount; ++i )
                {
                    var segment = (ZeldaPathSegment)waypointMap.GetPathSegmentAt( i );

                    context.Write( waypointMap.GetIndexOf( segment.From ) );
                    context.Write( waypointMap.GetIndexOf( segment.To ) );
                    context.Write( (byte)segment.PreferredWaypoint );

                    segment.CacheTilePath();
                    context.Write( segment.TileDistance );
                    segment.InvalidateCachedTilePath();
                }
            }

            /// <summary>
            /// Serializes the Paths of the given ZeldaWaypointMap using the given ISerializationContext.
            /// </summary>
            /// <param name="waypointMap">
            /// The object to serialize.
            /// </param>
            /// <param name="context">
            /// The context that provides everything required for the serialization process.
            /// </param>
            private static void SerializePaths( ZeldaWaypointMap waypointMap, ISerializationContext context )
            {
                const int CurrentVersion = 1;
                context.Write( CurrentVersion );
                context.Write( waypointMap.PathCount );

                for( int pathIndex = 0; pathIndex < waypointMap.PathCount; ++pathIndex )
                {
                    ZeldaPath path = waypointMap.GetPathAt( pathIndex );
                    
                    context.Write( path.Name ?? string.Empty );
                    context.Write( path.Length );

                    for( int waypointIndex = 0; waypointIndex < path.Length; ++waypointIndex )
                    {
                        context.Write( waypointMap.GetIndexOf( path[waypointIndex] ) );
                    }
                }
            }

            /// <summary>
            /// Deserializes the given object using the given IDeserializationContext.
            /// </summary>
            /// <param name="object">
            /// The object to deserialize.
            /// </param>
            /// <param name="context">
            /// The context that provides everything required for the deserialization process.
            /// </param>
            public override void Deserialize( ZeldaWaypointMap @object, IDeserializationContext context )
            {
                const int CurrentVersion = 2;
                int version = context.ReadInt32();
                ThrowHelper.InvalidVersion( version, CurrentVersion, "ZeldaWaypointMap" );

                DeserializeWaypoints( @object, context );
                DeserializeSegments( @object, context );
                DeserializePaths( @object, context );
            }

            /// <summary>
            /// Deserializes the Waypoints of the given ZeldaWaypointMap.
            /// </summary>
            /// <param name="waypointMap">
            /// The ZeldaWaypointMap to deserialize.
            /// </param>
            /// <param name="context">
            /// The context that provides everything required for the deserialization process.
            /// </param>
            private static void DeserializeWaypoints( ZeldaWaypointMap waypointMap, IDeserializationContext context )
            {
                const int CurrentVersion = 1;
                int version = context.ReadInt32();
                ThrowHelper.InvalidVersion( version, CurrentVersion, "ZeldaWaypointMap - Waypoints" );

                int waypointCount = context.ReadInt32();

                for( int index = 0; index < waypointCount; ++index )
                {
                    string name = context.ReadString();
                    int floorNumber = context.ReadInt32();
                    Vector2 position = context.ReadVector2();

                    var waypoint = (ZeldaWaypoint)waypointMap.AddWaypoint( position );
                    waypoint.Name = name;
                    waypoint.FloorNumber = floorNumber;
                }
            }

            /// <summary>
            /// Deserializes the PathSegments of the given ZeldaWaypointMap.
            /// </summary>
            /// <param name="waypointMap">
            /// The ZeldaWaypointMap to deserialize.
            /// </param>
            /// <param name="context">
            /// The context that provides everything required for the deserialization process.
            /// </param>
            private static void DeserializeSegments( ZeldaWaypointMap waypointMap, IDeserializationContext context )
            {
                const int CurrentVersion = 1;
                int version = context.ReadInt32();
                ThrowHelper.InvalidVersion( version, CurrentVersion, "ZeldaWaypointMap - Path Segments" );

                int segmentCount = context.ReadInt32();

                for( int index = 0; index < segmentCount; ++index )
                {
                    int fromIndex = context.ReadInt32();
                    int toIndex = context.ReadInt32();
                    var preferedWaypoint = (PathSegmentWaypoint)context.ReadByte();
                    float tileDistance = context.ReadSingle();

                    var segment = (ZeldaPathSegment)waypointMap.AddPathSegment(
                        waypointMap.GetWaypointAt( fromIndex ),
                        waypointMap.GetWaypointAt( toIndex )
                    );

                    segment.PreferredWaypoint = preferedWaypoint;
                    segment.TileDistance = tileDistance;
                }
            }

            /// <summary>
            /// Deserializes the Paths of the given ZeldaWaypointMap.
            /// </summary>
            /// <param name="waypointMap">
            /// The ZeldaWaypointMap to deserialize.
            /// </param>
            /// <param name="context">
            /// The context that provides everything required for the deserialization process.
            /// </param>
            private static void DeserializePaths( ZeldaWaypointMap waypointMap, IDeserializationContext context )
            {
                const int CurrentVersion = 1;
                int version = context.ReadInt32();
                ThrowHelper.InvalidVersion( version, CurrentVersion, "ZeldaWaypointMap - Paths" );

                int pathCount = context.ReadInt32();
                waypointMap.paths.Capacity = pathCount;

                for( int pathIndex = 0; pathIndex < pathCount; ++pathIndex )
                {
                    ZeldaPath path = waypointMap.AddPath();
                    path.Name = context.ReadString();

                    int pathLength = context.ReadInt32();
                    path.Capacity = pathLength;

                    for( int j = 0; j < pathLength; ++j )
                    {
                        int actualIndex = context.ReadInt32();
                        Waypoint waypoint = waypointMap.GetWaypointAt( actualIndex );

                        path.Add( waypoint );
                    }
                }
            }
        }
    }
}
