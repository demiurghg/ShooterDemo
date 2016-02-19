using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fusion.Engine.Common;


namespace ShooterDemo.Core {
	public abstract class EntityController<T> : IEntityController {

		public readonly Game Game;
		public readonly World World;

		Dictionary<uint, T> dictionary;

		/// <summary>
		/// Delegate used for object iteration.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="obj"></param>
		protected delegate void IterateAction ( ref Entity entity, T obj );


		/// <summary>
		/// 
		/// </summary>
		/// <param name="game"></param>
		public EntityController ( World world )
		{
			dictionary	=	new Dictionary<uint,T>();

			World	=	world;
			Game	=	world.Game;
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
				action( ref World.Entities[item.Key], item.Value );
			}
		}



		/// <summary>
		/// Updates controller.
		/// </summary>
		/// <param name="gameTime"></param>
		public virtual void Update ( GameTime gameTime ) {}

		/// <summary>
		/// Called when entity has died.
		/// </summary>
		/// <param name="id"></param>
		public virtual void Kill ( uint id ) {}
	}
}
