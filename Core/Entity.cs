using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fusion.Core.Mathematics;

namespace ShooterDemo.Core {
	public class Entity {

		/// <summary>
		/// Players guid.
		/// </summary>
		public Guid PlayerGuid;
		
		/// <summary>
		///	Gets non-zero unique entity's ID.
		///	Zero ID idicates that entity is dead.
		/// </summary>
		public uint UniqueID { get; private set; }

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
		/// Teleport counter 0
		/// </summary>
		public ushort TeleportCounter;

		

	}
}
