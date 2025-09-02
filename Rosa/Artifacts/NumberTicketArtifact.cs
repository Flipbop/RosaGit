using Nanoray.PluginManager;
using Nickel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Flipbop.Rosa;

internal sealed class NumberTicketArtifact : Artifact, IRegisterable
{
	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		helper.Content.Artifacts.RegisterArtifact("NumberTicket", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.RosaDeck.Deck,
				pools = ModEntry.GetArtifactPools(MethodBase.GetCurrentMethod()!.DeclaringType!)
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/Artifacts/NumberTicket.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "NumberTicket", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "NumberTicket", "description"]).Localize
		});
	}
	
	
	/*public override List<Tooltip>? GetExtraTooltips()
		=> [new Tooltip()];*/


	public override void OnDrawCard(State state, Combat combat, int count)
	{
		base.OnDrawCard(state, combat, count);
		if (combat.hand.Count >9)
		{
			combat.Queue(new AStatus() {status = Status.drawNextTurn, statusAmount = 1, targetPlayer = true});
		}
	}
}
