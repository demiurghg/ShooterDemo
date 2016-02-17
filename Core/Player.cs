using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShooterDemo.Core {
	public struct Player {

		/// <summary>
		/// Client's GUID.
		/// </summary>
		public Guid ClientGuid;

		/// <summary>
		/// Controlled entity ID.
		/// </summary>
		public uint EntityID;

		/// <summary>
		/// Score
		/// </summary>
		public int Score;


		/// <summary>
		/// 
		/// </summary>
		/// <param name="entityId"></param>
		public void Initialize ( Guid clientGuid, uint entityId )
		{
			ClientGuid	=	clientGuid;
			EntityID	=	entityId;
			Score		=	0;
		}
	}
}
