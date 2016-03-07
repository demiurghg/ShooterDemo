using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShooterDemo.Core {
	public enum Inventory : byte {

		Health,
		Armor,

		Machinegun,
		Shotgun,
		SuperShotgun,
		GrenadeLauncher,
		RocketLauncher,
		HyperBlaster,
		Chaingun,
		Railgun,
		BFG,
		
		Bullets,
		Shells,
		Grenades,
		Rockets,
		Cells,
		Slugs,

		WeaponCooldown,
		QuadDamage,

		Max,
	}
}
