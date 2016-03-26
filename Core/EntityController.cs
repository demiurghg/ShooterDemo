using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fusion.Engine.Common;
using Fusion.Core.Mathematics;


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
		protected delegate void IterateAction ( bool dirty, Entity entity, T obj );
		protected delegate void ApplyAction ( Entity entity, T obj );


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
		/// <param name="damage"></param>
		/// <param name="kickImpulse"></param>
		/// <param name="kickPoint"></param>
		/// <param name="damageType"></param>
		public virtual bool Damage ( uint targetID, uint attackerID, short damage, Vector3 kickImpulse, Vector3 kickPoint, DamageType damageType )
		{
			return false;
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
		/// <param name="id"></param>
		/// <returns></returns>
		protected T GetObject ( uint id )
		{
			T obj;
			if (dictionary.TryGetValue(id, out obj)) {
				return obj;
			} else {
				return default(T);
			}
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <param name="action"></param>
		protected void ApplyToObject ( uint id, ApplyAction action )
		{
			Entity e = World.GetEntity(id);

			T obj;
			if (e!=null && dictionary.TryGetValue(id, out obj)) {
				action( e, obj );
			}
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="?"></param>
		protected void IterateObjects ( bool dirty, IterateAction action )
		{
			foreach ( var item in dictionary ) {
				action( dirty, World.GetEntity(item.Key), item.Value );
			}
		}



		/// <summary>
		/// Updates controller.
		/// </summary>
		/// <param name="gameTime"></param>
		public virtual void Update ( float elapsedTime, bool dirty ) {}

		/// <summary>
		/// Called when entity has died.
		/// </summary>
		/// <param name="id"></param>
		public virtual void Kill ( uint id ) {}
	}
}
