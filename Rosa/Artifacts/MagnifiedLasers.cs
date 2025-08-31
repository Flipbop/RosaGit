using Nanoray.PluginManager;
using Nickel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Flipbop.Cleo;

internal sealed class MagnifiedLasersArtifact : Artifact, IRegisterable
{
	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		helper.Content.Artifacts.RegisterArtifact("MagnifiedLasers", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.CleoDeck.Deck,
				pools = ModEntry.GetArtifactPools(MethodBase.GetCurrentMethod()!.DeclaringType!)
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/Artifacts/MagnifiedLasers.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "MagnifiedLasers", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "MagnifiedLasers", "description"]).Localize
		});
	}
	public override int ModifyBaseDamage( int baseDamage, Card? card, State state, Combat? combat, bool fromPlayer)
	{
		if (combat?.hand?.Count == 0) return 0; 
		if (combat?.hand != null && combat.hand[^1].upgrade != Upgrade.None && card?.Equals(combat.hand[^1]) == true)
		{
			return 1;
		}
		return 0;
	}
}
