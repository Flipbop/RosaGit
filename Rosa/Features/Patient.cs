using Nickel;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace Flipbop.Rosa;

internal static class PatientExt
{
	public static bool GetIsPatient(this Card self)
		=> ModEntry.Instance.Helper.ModData.GetModDataOrDefault<bool>(self, "Patient");

	public static void SetIsPatient(this Card self, bool value)
		=> ModEntry.Instance.Helper.ModData.SetModData(self, "Patient", value);

}

internal sealed class PatientManager
{
	internal static readonly ICardTraitEntry Trait = ModEntry.Instance.PatientTrait;
	
	public PatientManager()
	{
		ModEntry.Instance.Harmony.Patch(
			original: AccessTools.DeclaredMethod(typeof(AEndTurn), nameof(AEndTurn.Begin)),
			prefix: new HarmonyMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(AEndTurn_Begin_Prefix))
		);
	}
	
	private static void AEndTurn_Begin_Prefix(State s, Combat c)
	{
		if (c.cardActions.Any(a => a is AEndTurn))
			return;
		foreach (Card card in c.hand)
		{
			if (card.GetIsPatient()) card.discount -= 1;
		} 

	}
}
