using Nanoray.PluginManager;
using Nickel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;

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
		
		ModEntry.Instance.Harmony.Patch(
			original: AccessTools.DeclaredMethod(typeof(ADrawCard), nameof(ADrawCard.Begin)),
			prefix: new HarmonyMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(ADraw_Begin_Prefix)),
			postfix: new HarmonyMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(ADraw_Begin_Postfix))
		);
	}


	public override List<Tooltip>? GetExtraTooltips()
		=> [..StatusMeta.GetTooltips(Status.drawNextTurn, 1)];


	public override void OnDrawCard(State state, Combat combat, int count)
	{
		base.OnDrawCard(state, combat, count);
		if (combat.hand.Count >9 && state.EnumerateAllArtifacts().Any((a) => a is NumberTicketArtifact))
		{
			combat.Queue(new AStatus() {status = Status.drawNextTurn, statusAmount = 1, targetPlayer = true});
		}
	}

	private static void ADraw_Begin_Prefix(ADrawCard __instance, Combat c, out int __state)
	{
		if (c.hand.Count + __instance.count > 10)
		{
			__state = c.hand.Count + __instance.count - 10;
		}
		else
		{
			__state = 0;
		}
	}

	private static void ADraw_Begin_Postfix(Combat c, in int __state)
	{
		c.Queue(new AStatus() {status = Status.drawNextTurn, statusAmount = __state, targetPlayer = true});
	}
}
