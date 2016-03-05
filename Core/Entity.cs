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
		/// Entity position
		/// </summary>
		public Vector3 PositionOld;

		/// <summary>
		/// Entity's angle
		/// </summary>
		public Quaternion Rotation;

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
		public Entity ( uint id, uint prefabId, uint parentId, Vector3 position, float yaw )
		{
			this.ID		=	id;
			UserGuid	=	new Guid();
			PrefabID	=	prefabId;
			ParentID	=	parentId;

			Rotation		=	Quaternion.RotationYawPitchRoll( yaw, 0, 0 );
			UserCtrlFlags	=	UserCtrlFlags.None;
			Position		=	position;
			PositionOld		=	position;
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
			writer.Write( Rotation );
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
			Rotation			=	reader.Read<Quaternion>();	
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
		public Matrix GetWorldMatrix (float lerpFactor)
		{
			return Matrix.RotationQuaternion( Rotation ) 
					* Matrix.Translation( LerpPosition(lerpFactor) );
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="lerpFactor"></param>
		/// <returns></returns>
		public Vector3 LerpPosition ( float lerpFactor )
		{
			//return Position;
			return Vector3.Lerp( PositionOld, Position, MathUtil.Clamp(lerpFactor,0,2f) );
		}
	}
}
