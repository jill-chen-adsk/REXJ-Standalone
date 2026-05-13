using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;

namespace ADSK.ViewExtension.TenkaiView.DialogItem
{
    public class ItmRoom
    {
        private readonly Room m_Room;

        public ItmRoom(Room rm)
        {
            m_Room = rm;
        }

        public ElementId RoomId => m_Room.Id;

        public string RoomName => m_Room.get_Parameter(BuiltInParameter.ROOM_NAME).AsString();

        public override string ToString() => RoomName;
    }
}
