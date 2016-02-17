using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fusion.Core.Mathematics;
using System.IO;

namespace ShooterDemo.Core {
	public struct Entity {

		/// <summary>
		///	Gets non-zero unique entity's ID.
		///	Zero ID idicates that entity is dead.
		/// </summary>
		public uint UniqueID;// { get; private set; }

		/// <summary>
		/// Players guid. Zero if no player.
		/// </summary>
		public Guid UserGuid;// { get; private set; }

		/// <summary>
		///	Gets parent's ID. 
		///	Zero value means no parent.
		/// </summary>
		public uint ParentID;// { get; private set; }

		/// <summary>
		/// Gets prefab ID.
		/// </summary>
		public uint PrefabID;// { get; private set; }


		/// <summary>
		/// Entity position
		/// </summary>
		public Vector3 Position;

		/// <summary>
		///	Entity old position
		/// </summary>
		public Vector3 PositionOld;

		/// <summary>
		/// Entity's angle
		/// </summary>
		public Angles Angles;

		/// <summary>
		/// Movement lerp factor
		/// </summary>
		public float LerpFactor;


		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		public void Initialize ( uint prefabId, uint id, uint parentId, Vector3 position, Angles angles )
		{
			UserGuid	=	new Guid();
			UniqueID	=	id;
			PrefabID	=	prefabId;
			ParentID	=	parentId;

			Angles		=	angles;
			Position	=	position;
			PositionOld	=	position;
			LerpFactor	=	0;
		}




		public void Kill ()
		{
			UserGuid	=	new Guid();
			UniqueID	=	0;
			PrefabID	=	0;
			ParentID	=	0;

			Angles		=	new Angles();
			Position	=	Vector3.Zero;
			PositionOld	=	Vector3.Zero;
			LerpFactor	=	0;
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		public void Write ( BinaryWriter writer )
		{
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		public void Read ( BinaryReader reader )
		{
		}
	}
}
