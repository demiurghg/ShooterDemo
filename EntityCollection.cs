using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fusion;
using Fusion.Core.Mathematics;
using Fusion.Core.Configuration;
using Fusion.Core.Shell;
using Fusion.Core.Utils;
using Fusion.Engine.Common;
using Fusion.Engine.Client;
using Fusion.Engine.Server;
using Fusion.Engine.Graphics;
using Fusion.Engine.Audio;
using Fusion.Engine.Input;

namespace ShooterDemo {

	/// <summary>
	/// Represents collection of game entities.
	/// </summary>
	public class EntityCollection : ICollection<Entity> {

		readonly HashSet<Entity> entities;
		readonly GameWorld gameWorld;


		/// <summary>
		/// 
		/// </summary>
		/// <param name="capacity"></param>
		public EntityCollection ( GameWorld gameWorld )
		{										 
			this.gameWorld	=	gameWorld;
			this.entities	=	new HashSet<Entity>();
		}
			


		/// <summary>
		/// Gets entity by its id.
		/// </summary>
		/// <param name="id"></param>
		/// <returns>Null if collection does not contain entity with given ID.</returns>
		public Entity this[ int id ] {
			get {
				return entities.SingleOrDefault( e => e.ID == id );
			}
		}



		/// <summary>
		/// Indicates that collection contains entity with given ID.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public bool ContainsID ( int id ) 
		{
			return entities.Any( e => e.ID == id );
		}



		/// <summary>
		/// Gets the number of elements contained in the GameEntityCollection.
		/// </summary>
		public int Count {
			get {
				return entities.Count;
			}
		}



		/// <summary>
		/// Gets a value indicating whether the GameEntityCollection is read-only.
		/// </summary>
		public bool IsReadOnly {
			get {
				return false;
			}
		}


		
		/// <summary>
		/// 
		/// This method is deferred. To apply call Commit.
		/// </summary>
		/// <param name="entity"></param>
		public void Add ( Entity entity )
		{
			if (!entities.Add( entity )) {
				Log.Warning("Can not add entity, it is alrady added");
			}
			entity.Activate( gameWorld );
		}



		/// <summary>
		/// Removes all items from the GameEntityCollection.
		/// This method is deferred. To apply call Commit.
		/// </summary>
		public void Clear ()
		{
			entities.Clear();
		}



		/// <summary>
		/// Removes the first occurrence of a specific object from the GameEntityCollection.
		/// This method is deferred. To apply call Commit.
		/// </summary>
		/// <param name="entity"></param>
		public bool Remove ( Entity entity )
		{
			if (entities.Remove(entity)) {
				entity.Deactivate(gameWorld);
				return true;
			} else {
				return false;
			}
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="predicated"></param>
		/// <returns></returns>
		public int RemoveAll ( Func<Entity,bool> predicate )
		{
			var toRemove = entities.Where ( e => predicate(e) );
			int count = 0;

			foreach ( var ent in toRemove ) {
				if (Remove( ent )) {
					count ++;
				}
			}

			return count;
		}



		/// <summary>
		/// Determines whether the GameEntityCollection contains a specific value.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public bool Contains ( Entity entity )
		{
			return entities.Contains( entity );
		}



		/// <summary>
		/// Copies the elements of the GameEntityCollection to an System.Array, starting at a particular System.Array index.
		///</summary>
		/// <param name="array"></param>
		/// <param name="arrayIndex"></param>
		public void CopyTo ( Entity[] array, int arrayIndex )
		{
			entities.CopyTo( array, arrayIndex );
		}



		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<Entity> GetEnumerator ()
		{
			return entities.GetEnumerator();
		}



		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns></returns>
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return entities.GetEnumerator();
		}

	}
}
