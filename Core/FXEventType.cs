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

	public enum FXEventType : uint {
		
		PlayerFootStep	,
		PlayerLanding	,
		PlayerFall		,
		PlayerJump		,
		PlayerPain1		,
		PlayerPain2		,
		PlayerPain3		,
		PlayerDeath		,

		BulletTrail		,
		GunMuzzle		,

		Explosion		,
	}
}
