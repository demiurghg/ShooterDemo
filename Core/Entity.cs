using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fusion;
using Fusion.Core;
using Fusion.Core.Utils;
using Fusion.Core.Mathematics;
using System.IO;

namespace ShooterDemo.Core {
	public class Entity {

		public Entity RemoteEntity = null;

		/// <summary>
		/// Entity ID
		/// </summary>
		public readonly uint ID;

		/// <summary>
		/// Client's lag.
		/// </summary>
		public float Lag = 0;

		/// <summary>
		/// Players guid. Zero if no player.
		/// </summary>
		public Guid UserGuid;// { get; private set; }


		/// <summary>
		///	Gets parent's ID. 
		///	Zero value means no parent.
		/// </summary>
		public uint ParentID { get; private set; }

		/// <summary>
		/// Gets prefab ID.
		/// </summary>
		public uint PrefabID { get; private set; }

		/// <summary>
		/// Entity position
		/// </summary>
		public Vector3 Position;

		/// <summary>
		/// Entity's angle
		/// </summary>
		public Angles Angles;

		/// <summary>
		/// Control flags.
		/// </summary>
		public UserCtrlFlags UserCtrlFlags;

		/// <summary>
		/// Linear object velocity
		/// </summary>
		public Vector3 LinearVelocity;

		/// <summary>
		/// Angular object velocity
		/// </summary>
		public Vector3 AngularVelocity;



		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		public Entity ( uint id )
		{
			ID	=	id;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		public Entity ( uint id, uint prefabId, uint parentId, Vector3 position, Angles angles )
		{
			this.ID		=	id;
			UserGuid	=	new Guid();
			PrefabID	=	prefabId;
			ParentID	=	parentId;

			Angles			=	angles;
			UserCtrlFlags	=	UserCtrlFlags.None;
			Position		=	position;
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		public void Write ( BinaryWriter writer )
		{
			writer.Write( Lag );
			writer.Write( UserGuid.ToByteArray() );
			writer.Write( ParentID );
			writer.Write( PrefabID );

			writer.Write( Position );
			writer.Write( Angles );
			writer.Write( (int)UserCtrlFlags );
			writer.Write( LinearVelocity );
			writer.Write( AngularVelocity );
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		public void Read ( BinaryReader reader )
		{
			Lag					=	reader.ReadSingle();
			UserGuid			=	new Guid( reader.ReadBytes(16) );
									
			ParentID			=	reader.ReadUInt32();
			PrefabID			=	reader.ReadUInt32();

			Position			=	reader.Read<Vector3>();	
			Angles				=	reader.Read<Angles>();	
			UserCtrlFlags		=	(UserCtrlFlags)reader.ReadInt32();
			LinearVelocity		=	reader.Read<Vector3>();
			AngularVelocity		=	reader.Read<Vector3>();	
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="prefabName"></param>
		/// <returns></returns>
		public bool Is ( string prefabName )
		{
			return ( PrefabID == Factory.GetPrefabID(prefabName) );
		}



		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public Matrix GetWorldMatrix ()
		{
			return Matrix.RotationYawPitchRoll( Angles.Yaw.Radians, Angles.Pitch.Radians, Angles.Roll.Radians ) * Matrix.Translation( Position );
		}
	}
}
