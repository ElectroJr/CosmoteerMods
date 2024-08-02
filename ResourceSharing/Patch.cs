using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using Cosmoteer.Game.Gui.Resources;
using Cosmoteer.Game.Multiplayer.MPInputs;
using Cosmoteer.Ships.Commands;
using Cosmoteer.Ships.Crew.Jobs;
using HarmonyLib;

namespace ResourceSharing;

/// <summary>
/// This patch redirects various calls to <see cref="CommandManager.CanExecuteCrewCommandsFromLocalPlayer"/> and
/// related methods in various resource transfer related code snippets to custom methods that are more permissive.
/// </summary>
[HarmonyPatch]
public static class Patch
{
	[HarmonyTranspiler]
	[HarmonyPatch(typeof(CrewAndResourceTransferWindow), nameof(CrewAndResourceTransferWindow.OnUpdatingUIState))]
	public static IEnumerable<CodeInstruction> UiStateTranspiler(IEnumerable<CodeInstruction> instructions)
	{
		return ReplaceCalls(instructions);
	}

	[HarmonyTranspiler]
	[HarmonyPatch(typeof(CrewAndResourceTransferWindow), nameof(CrewAndResourceTransferWindow.CancelTransfers))]
	public static IEnumerable<CodeInstruction> CancelTranspiler(IEnumerable<CodeInstruction> instructions)
	{
		return ReplaceCalls(instructions);
	}

	[HarmonyTranspiler]
	[HarmonyPatch(typeof(MPCancelMoveResourcesInput), nameof(MPCancelMoveResourcesInput.Execute))]
	public static IEnumerable<CodeInstruction> CancelExecuteTranspiler(IEnumerable<CodeInstruction> instructions)
	{
		return ReplaceCalls(instructions);
	}

	[HarmonyTranspiler]
	[HarmonyPatch(typeof(MPSetCrewHomeShipInput), nameof(MPSetCrewHomeShipInput.Execute))]
	public static IEnumerable<CodeInstruction> CrewHomeTranspiler(IEnumerable<CodeInstruction> instructions)
	{
		return ReplaceCalls(instructions);
	}

	[HarmonyTranspiler]
	[HarmonyPatch(typeof(MPMoveResourcesInput), nameof(MPMoveResourcesInput.Execute))]
	public static IEnumerable<CodeInstruction> ResourceTranspiler(IEnumerable<CodeInstruction> instructions)
	{
		return ReplaceCalls(instructions);
	}

	[HarmonyTranspiler]
	[HarmonyPatch(typeof(CrewAndResourceTransferWindow), nameof(CrewAndResourceTransferWindow.IsValidSendShip))]
	public static IEnumerable<CodeInstruction> ValidSendTranspiler(IEnumerable<CodeInstruction> instructions)
	{
		return ReplaceCalls(instructions);
	}

	[HarmonyTranspiler]
	[HarmonyPatch(typeof(ResourceTransferJob), nameof(ResourceTransferJob.IsSourceValid))]
	public static IEnumerable<CodeInstruction> TransferJobTranspiler(IEnumerable<CodeInstruction> instructions)
	{
		return ReplaceCalls(instructions);
	}

	public static List<CodeInstruction> ReplaceCalls(IEnumerable<CodeInstruction> instructions)
	{
		var replacement1 = typeof(Patch).GetMethod(nameof(CanExecuteCommandsFromLocalPlayer))!;
		var replacement2 = typeof(Patch).GetMethod(nameof(CanExecuteCrewCommandsFromLocalPlayer))!;
		var replacement3 = typeof(Patch).GetMethod(nameof(IsCommandableByLocalPlayer))!;
		var replacement4 = typeof(Patch).GetMethod(nameof(CanExecuteCrewCommandsFrom))!;
		var replacement5 = typeof(Patch).GetMethod(nameof(IsCommandableBy))!;

		var target1 = typeof(CommandManager).GetProperty(nameof(CommandManager.CanExecuteCommandsFromLocalPlayer))
			?.GetGetMethod();
		if (target1 == null)
			throw new Exception(
				$"Resource Sharing Mod: Cannot find {nameof(CommandManager.CanExecuteCommandsFromLocalPlayer)} method");

		var target2 = typeof(CommandManager).GetProperty(nameof(CommandManager.CanExecuteCrewCommandsFromLocalPlayer))
			?.GetGetMethod();
		if (target2 == null)
			throw new Exception(
				$"Resource Sharing Mod: Cannot find {nameof(CommandManager.CanExecuteCrewCommandsFromLocalPlayer)} method");

		var target3 = typeof(CommandManager).GetProperty(nameof(CommandManager.IsCommandableByLocalPlayer))
			?.GetGetMethod();
		if (target3 == null)
			throw new Exception(
				$"Resource Sharing Mod: Cannot find {nameof(CommandManager.IsCommandableByLocalPlayer)} method");

		var target4 = typeof(CommandManager).GetMethod(nameof(CommandManager.CanExecuteCrewCommandsFrom));
		if (target4 == null)
			throw new Exception(
				$"Resource Sharing Mod: Cannot find {nameof(CommandManager.CanExecuteCrewCommandsFrom)} method");

		var target5 = typeof(CommandManager).GetMethod(nameof(CommandManager.IsCommandableBy));
		if (target5 == null)
			throw new Exception($"Resource Sharing Mod: Cannot find {nameof(CommandManager.IsCommandableBy)} method");

		var codes = new List<CodeInstruction>(instructions);
		ReplaceCalls(codes, target1, replacement1);
		ReplaceCalls(codes, target2, replacement2);
		ReplaceCalls(codes, target3, replacement3);
		ReplaceCalls(codes, target4, replacement4);
		ReplaceCalls(codes, target5, replacement5);
		return codes;
	}

	public static void ReplaceCalls(List<CodeInstruction> codes, MethodInfo target, MethodInfo replacement)
	{
		foreach (ref var code in CollectionsMarshal.AsSpan(codes))
		{
			if (code.opcode != OpCodes.Callvirt)
				continue;

			if (code.operand is not MethodInfo original || original != target)
				continue;

			code.opcode = OpCodes.Call;
			code.operand = replacement;
		}
	}

	public static bool CanExecuteCommandsFromLocalPlayer(CommandManager @this)
	{
		return @this.Game != null && (@this.CanExecuteCommandsFrom(@this.Game.LocalPlayerIndex) ||
		                              @this.Ship.IsAlliesWithLocalPlayer);
	}

	public static bool CanExecuteCrewCommandsFromLocalPlayer(CommandManager @this)
	{
		return @this.Game != null && (@this.CanExecuteCrewCommandsFrom(@this.Game.LocalPlayerIndex) ||
		                              @this.Ship.IsAlliesWithLocalPlayer);
	}

	public static bool IsCommandableByLocalPlayer(CommandManager @this)
	{
		return @this.Game != null &&
		       (@this.IsCommandableBy(@this.Game.LocalPlayerIndex) || @this.Ship.IsAlliesWithLocalPlayer);
	}

	public static bool CanExecuteCrewCommandsFrom(CommandManager @this, int playerIndex)
	{
		return @this.Ship.Crew.Assigned.Count > 0 &&
		       (@this.IsCommandableBy(playerIndex) || @this.Ship.IsAlliesWith(playerIndex));
	}

	public static bool IsCommandableBy(CommandManager @this, int playerIndex)
	{
		return playerIndex == -4 || playerIndex == @this.Ship.Metadata.PlayerIndex ||
		       @this.Ship.IsAlliesWith(playerIndex);
	}
}