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
		/// Players guid. Zero if no player.
		/// </summary>
		public Guid UserGuid;// { get; private set; }

		/// <summary>
		/// Gets entity state
		/// </summary>
		public EntityState State;

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
		/// Teleportation counter.
		/// Used to prevent interpolation in discreete movement.
		/// </summary>
		public byte TeleportCount;

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
		/// Entity's angle
		/// </summary>
		public Quaternion RotationOld;

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
		/// Inventory
		/// </summary>
		readonly short[] inventory = new short[(byte)Inventory.Max];

		/// <summary>
		/// Currently active item.
		/// </summary>
		public Inventory ActiveItem;


		public Vector3 KickImpulse;
		public Vector3 KickPoint;



		/// <summary>
		/// Used to replicate entity on client side.
		/// </summary>
		/// <param name="id"></param>
		public Entity ( uint id )
		{
			ID	=	id;
			RotationOld		=	Quaternion.Identity;
			Rotation		=	Quaternion.Identity;
			TeleportCount	=	0xFF;
		}


		/// <summary>
		/// Used to spawn entity on server side.
		/// </summary>
		/// <param name="id"></param>
		public Entity ( uint id, uint prefabId, uint parentId, Vector3 position, float yaw )
		{
			this.ID		=	id;

			TeleportCount	=	0;

			RotationOld		=	Quaternion.Identity;
			PositionOld		=	Vector3.Zero;

			UserGuid		=	new Guid();
			PrefabID		=	prefabId;
			ParentID		=	parentId;

			Rotation		=	Quaternion.RotationYawPitchRoll( yaw, 0, 0 );
			UserCtrlFlags	=	UserCtrlFlags.None;
			Position		=	position;
			PositionOld		=	position;
		}



		/// <summary>
		/// Immediatly put entity in given position without interpolation :
		/// </summary>
		/// <param name="position"></param>
		/// <param name="orient"></param>
		void SetPose ( Vector3 position, Quaternion orient )
		{
			TeleportCount++;
			TeleportCount &= 0x7F;

			Position		=	position;
			Rotation		=	orient;
			PositionOld		=	position;
			RotationOld		=	orient;
		}



		public void SetItemCount ( Inventory item, short count ) 
		{
			inventory[(byte)item] = count;
		}


		public short GetItemCount ( Inventory item ) 
		{
			return inventory[(byte)item];
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="impulse"></param>
		/// <param name="wsPoint"></param>
		public void Kick ( Vector3 impulse, Vector3 wsPoint )
		{
			this.KickImpulse	=	impulse;
			this.KickPoint		=	wsPoint;
		}


		/// <summary>
		/// Moves entity to given position with interpolation :
		/// </summary>
		/// <param name="position"></param>
		/// <param name="orient"></param>
		void Move ( Vector3 position, Quaternion orient )
		{
			Position		=	position;
			Rotation		=	orient;
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		public void Write ( BinaryWriter writer )
		{
			writer.Write( UserGuid.ToByteArray() );

			writer.Write( ParentID );
			writer.Write( PrefabID );
			writer.Write( (int)State );

			writer.Write( TeleportCount );

			writer.Write( Position );
			writer.Write( Rotation );
			writer.Write( (int)UserCtrlFlags );
			writer.Write( LinearVelocity );
			writer.Write( AngularVelocity );

			for (int i=0; i<inventory.Length; i++) {
				writer.Write( inventory[i] );
			}

			writer.Write( (byte)ActiveItem );
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		public void Read ( BinaryReader reader, float lerpFactor )
		{
			//	keep old teleport counter :
			var oldTeleport	=	TeleportCount;

			//	set old values :
			PositionOld		=	LerpPosition( lerpFactor );
			RotationOld		=	LerpRotation( lerpFactor );

			//	read state :
			UserGuid		=	new Guid( reader.ReadBytes(16) );
								
			ParentID		=	reader.ReadUInt32();
			PrefabID		=	reader.ReadUInt32();
			State			=	(EntityState)reader.ReadInt32();

			TeleportCount	=	reader.ReadByte();

			Position		=	reader.Read<Vector3>();	
			Rotation		=	reader.Read<Quaternion>();	
			UserCtrlFlags	=	(UserCtrlFlags)reader.ReadInt32();
			LinearVelocity	=	reader.Read<Vector3>();
			AngularVelocity	=	reader.Read<Vector3>();	


			for (int i=0; i<inventory.Length; i++) {
				inventory[i]	=	reader.ReadInt16();
			}

			ActiveItem	=	(Inventory)reader.ReadByte();


			//	entity teleported - reset position and rotation :
			if (oldTeleport!=TeleportCount) {
				PositionOld	=	Position;
				RotationOld	=	Rotation;
			}
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
			return Matrix.RotationQuaternion( LerpRotation(lerpFactor) ) 
					* Matrix.Translation( LerpPosition(lerpFactor) );
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="lerpFactor"></param>
		/// <returns></returns>
		public Quaternion LerpRotation ( float lerpFactor )
		{
			//return Position;
			return Quaternion.Slerp( RotationOld, Rotation, MathUtil.Clamp(lerpFactor,0,1f) );
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
