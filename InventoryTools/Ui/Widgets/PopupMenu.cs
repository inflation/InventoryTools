using System;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using InventoryTools.Localizers;

namespace InventoryTools.Ui.Widgets;

public class PopupMenu
{
    private readonly UiText _text;
    private List<IPopupMenuItem> _items;
    private PopupMenuButtons _openButtons;
    private string _id;

    public List<IPopupMenuItem> Items => _items;

    public enum PopupMenuButtons
    {
        Left,
        Right,
        Middle,
        LeftRight,
        All
    }

    public interface IPopupMenuItem
    {
        public void Draw(UiText text);
        public string? DrawPopup(UiText text);
        public Func<bool>? ShouldDraw { get; }
    }

    public class PopupMenuItemSelectable : IPopupMenuItem
    {
        private string _name;
        private string _id;
        private string? _tooltip;
        private Action<string>? _callback { get; }
        public Func<bool>? ShouldDraw { get; }

        public PopupMenuItemSelectable(string name, string id, Action<string>? callback, string? tooltip = null, Func<bool>? shouldDraw = null)
        {
            _tooltip = tooltip;
            _name = name;
            _id = id;
            _callback = callback;
            ShouldDraw = shouldDraw;
        }

        public void Draw(UiText text)
        {

        }

        public string? DrawPopup(UiText text)
        {
            if (ImGui.Selectable(text.Label(_name)))
            {
                _callback?.Invoke(_id);
                return null;
            }

            if (_tooltip != null)
            {
                OtterGui.ImGuiUtil.HoverTooltip(text.Get(_tooltip));
            }

            return null;
        }

    }

    public class PopupMenuItemSelectableAskName : IPopupMenuItem
    {
        private string _name;
        private string _id;
        private Action<string, string>? _callback { get; }
        private string _newName = "";
        private string _popupName;
        private string _defaultName;
        private string? _tooltip;
        public Func<bool>? ShouldDraw { get; }

        public PopupMenuItemSelectableAskName(string name, string id, string defaultName,
            Action<string, string>? callback, string? tooltip = null, Func<bool>? shouldDraw = null)
        {
            _defaultName = defaultName;
            _newName = defaultName;
            _name = name;
            _id = id;
            _callback = callback;
            _tooltip = tooltip;
            _popupName = _id+"NF";
            ShouldDraw = shouldDraw;
        }

        public void Draw(UiText text)
        {
            if (ImGuiUtil.OpenNameField(_popupName, ref _newName))
            {
                _callback?.Invoke(_newName, _id);
                _newName = _defaultName;;
            }
        }

        public string? DrawPopup(UiText text)
        {
            if (ImGui.Selectable(text.Label(_name)))
            {
                _newName = _defaultName;
                return _popupName;
            }

            if (_tooltip != null)
            {
                OtterGui.ImGuiUtil.HoverTooltip(text.Get(_tooltip));
            }

            return null;
        }
    }

    public class PopupMenuItemSelectableConfirm : IPopupMenuItem
    {
        private string _name;
        private string _id;
        private Action<string, bool>? _callback { get; }
        private string _popupName;
        private string _question;
        private string? _tooltip;

        public PopupMenuItemSelectableConfirm(string name, string id, string question,
            Action<string, bool>? callback, string? tooltip = null, Func<bool>? shouldDraw = null)
        {
            _question = question;
            _name = name;
            _id = id;
            _callback = callback;
            _tooltip = tooltip;
            _popupName = _id+"NF";
            ShouldDraw = shouldDraw;
        }

        public void Draw(UiText text)
        {
            using var popup = ImRaii.Popup(_popupName);
            if (!popup)
                return ;

            ImGui.TextUnformatted(
                text.Get(_question) + "\n" + text.Get("This operation cannot be undone!") + "\n\n");
            ImGui.Separator();

            if (ImGui.Button(text.Label("OK"), new Vector2(120, 0) * ImGui.GetIO().FontGlobalScale))
            {
                _callback?.Invoke(_id, true);
                ImGui.CloseCurrentPopup();
            }
            ImGui.SameLine();
            if (ImGui.Button(text.Label("Cancel"), new Vector2(120, 0) * ImGui.GetIO().FontGlobalScale))
            {
                _callback?.Invoke(_id, false);
                ImGui.CloseCurrentPopup();
            }

        }

        public string? DrawPopup(UiText text)
        {
            if (ImGui.Selectable(text.Label(_name)))
            {
                return _popupName;
            }

            if (_tooltip != null)
            {
                OtterGui.ImGuiUtil.HoverTooltip(text.Get(_tooltip));
            }

            return null;
        }

        public Func<bool>? ShouldDraw { get; }
    }

    public class PopupMenuItemSeparator : IPopupMenuItem
    {
        public void Draw(UiText text)
        {
        }

        public string? DrawPopup(UiText text)
        {
            ImGui.Separator();
            return null;
        }

        public Func<bool>? ShouldDraw { get; }
    }
    public PopupMenu(UiText text, string id, PopupMenuButtons openButtons,List<IPopupMenuItem> items)
    {
        _id = id;
        _text = text;
        _openButtons = openButtons;
        _items = items;
    }

    public void Open()
    {
        ImGui.OpenPopup("RightClick" + _id);
    }

    public void Draw()
    {
        var isMouseReleased = ImGui.IsMouseReleased(ImGuiMouseButton.Left) && _openButtons is PopupMenuButtons.All or PopupMenuButtons.Left or PopupMenuButtons.LeftRight ||
                              ImGui.IsMouseReleased(ImGuiMouseButton.Right) && _openButtons is PopupMenuButtons.All or PopupMenuButtons.Right or PopupMenuButtons.LeftRight ||
                              ImGui.IsMouseReleased(ImGuiMouseButton.Middle) && _openButtons is PopupMenuButtons.All or PopupMenuButtons.Middle;

        if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled & ImGuiHoveredFlags.AllowWhenOverlapped & ImGuiHoveredFlags.AllowWhenBlockedByPopup & ImGuiHoveredFlags.AllowWhenBlockedByActiveItem & ImGuiHoveredFlags.AnyWindow) && isMouseReleased)
        {
            ImGui.OpenPopup("RightClick" + _id);
        }

        string? newPopupName = null;
        using (var popup = ImRaii.Popup("RightClick"+ _id))
        {
            if (popup.Success)
            {
                foreach (var item in _items)
                {
                    if (item.ShouldDraw != null)
                    {
                        if (!item.ShouldDraw())
                        {
                            continue;
                        }
                    }
                    var drawPopup = item.DrawPopup(_text);
                    if (drawPopup != null)
                    {
                        newPopupName = drawPopup;
                    }
                }
            }
        }
        if (newPopupName != null)
        {
            ImGui.OpenPopup(newPopupName);
        }

        foreach (var item in _items)
        {
            item.Draw(_text);
        }
    }
}
