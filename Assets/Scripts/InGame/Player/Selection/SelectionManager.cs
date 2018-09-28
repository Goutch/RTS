using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization.Formatters;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;

namespace DefaultNamespace
{
    public delegate void SelectionEndEventHandler(Rect Bounds, Camera playerCam);

    public class SelectionManager : NetworkBehaviour
    {
        [SerializeField] private int maxSavedSelection = 10;
        [SerializeField] private Texture2D selectionHightlight;
        private SavedSelectionGrid savedSelections;
        private UnitGroup selectedUnits;

        public UnitGroup SelectedUnits
        {
            get { return selectedUnits; }
        }

        private Rect selectionRect;

        private Camera playerCam;

        private Vector2 beiginPosition;

        private BoxCollider2D collider;

        public event SelectionEndEventHandler OnSelectionEnd;


        private void Awake()
        {
            selectedUnits = new UnitGroup();
            savedSelections = GetComponentInChildren<SavedSelectionGrid>();
            beiginPosition = -Vector2.one;
            collider = GetComponent<BoxCollider2D>();
            playerCam = Camera.main;
        }


        public void BeginSelection(Vector2 position)
        {
            beiginPosition = position;
        }

        public void EndSelection(bool addToExistingSelection)
        {
            if (hasAuthority)
            {
                foreach (IUnit u in selectedUnits.Composants)
                {
                    u.SetSelected(false);
                }

                if (!addToExistingSelection)
                {
                    selectedUnits.Composants.Clear();
                }

                NormalizeBounds();
                beiginPosition = -Vector2.one;
                if (OnSelectionEnd != null) OnSelectionEnd(selectionRect, playerCam);
                foreach (IUnit u in selectedUnits.Composants)
                {
                    u.SetSelected(true);
                }
            }
        }

        public void SaveSelection(int index)
        {
            if (selectedUnits.Composants.Any())
                savedSelections.SaveSelection(selectedUnits, index);
        }

        public void SelectSavedSelection(int index)
        {
            if (savedSelections.IsGroupSaved(index))
            {
                foreach (IUnit u in selectedUnits.Composants)
                {
                    u.SetSelected(false);
                }

                selectedUnits = savedSelections.GetSelection(index);
                foreach (IUnit u in selectedUnits.Composants)
                {
                    u.SetSelected(true);
                }
            }
        }

        public void DrawSelectionSquare(Vector2 position)
        {
            NormalizeBounds();

            selectionRect = new Rect(beiginPosition.x, Screen.height - beiginPosition.y, position.x - beiginPosition.x,
                (Screen.height - position.y) - (Screen.height - beiginPosition.y));
        }

        private void OnGUI()
        {
            if (beiginPosition != -Vector2.one)
            {
                GUI.color = new Color(1, 1, 1, 0.5f);
                GUI.DrawTexture(selectionRect, selectionHightlight);
            }
        }

        private void NormalizeBounds()
        {
            if (selectionRect.width < 0)
            {
                selectionRect.x += selectionRect.width;
                selectionRect.width = -selectionRect.width;
            }

            if (selectionRect.height < 0)
            {
                selectionRect.y += selectionRect.height;
                selectionRect.height = -selectionRect.height;
            }
        }

        [Command]
        private void CmdSelectSavedSelection(int index)
        {
            if (!isLocalPlayer)
                if (savedSelections.IsGroupSaved(index))
                {
                    selectedUnits = savedSelections.GetSelection(index);
                }
        }

        [ClientRpc]
        private void RpcSelectSavedSelection(int index)
        {
            if (!isLocalPlayer)
                if (savedSelections.IsGroupSaved(index))
                {
                    selectedUnits = savedSelections.GetSelection(index);
                }
        }

        [Command]
        private void CmdSaveCurrentSelection(int index)
        {
            if (!isLocalPlayer)
                if (selectedUnits.Composants.Any())
                {
                    savedSelections.SaveSelection(selectedUnits, index);
                    RpcSaveCurrentSelection(index);
                }
        }

        [ClientRpc]
        private void RpcSaveCurrentSelection(int index)
        {
            if (!isLocalPlayer)
                if (selectedUnits.Composants.Any())
                    savedSelections.SaveSelection(selectedUnits, index);
        }
    }
}