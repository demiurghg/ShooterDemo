using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fusion.Engine.Common;


namespace ShooterDemo.Core {
	public abstract class EntityView<T> : IEntityView {

		public readonly Game Game;
		public readonly World World;

		Dictionary<uint, T> dictionary;

		/// <summary>
		/// Delegate used for object iteration.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="obj"></param>
		protected delegate void IterateAction ( Entity entity, T obj );


		/// <summary>
		/// 
		/// </summary>
		/// <param name="game"></param>
		public EntityView ( World world )
		{
			World	=	world;
			Game	=	world.Game;

			dictionary	=	new Dictionary<uint,T>();
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <param name="obj"></param>
		protected void AddObject ( uint id, T obj )
		{
			dictionary.Add( id, obj );
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		protected bool RemoveObject ( uint id, out T item )
		{
			if (dictionary.TryGetValue( id, out item )) {
				return dictionary.Remove( id );
			} else {
				return false;
			}
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="?"></param>
		protected void IterateObjects ( IterateAction action )
		{
			foreach ( var item in dictionary ) {
				action( World.GetEntity(item.Key), item.Value );
			}
		}


		/// <summary>
		/// Called on each viewable entity.
		/// </summary>
		/// <param name="entity"></param>
		public virtual void Update ( float elapsedTime ) {}

		/// <summary>
		/// Called when entity has died.
		/// </summary>
		/// <param name="id"></param>
		public virtual void Kill ( uint id ) {}
	}
}
