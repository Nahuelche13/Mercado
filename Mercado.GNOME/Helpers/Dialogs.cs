namespace Mercado.GNOME.Helpers;

internal static class SQLErrorCodes {
    public static Adw.MessageDialog FillDialogSQLErrorCodes(Gtk.Window? parent, int error, string sucessMessage) {
        Adw.MessageDialog dialog = Adw.MessageDialog.New(parent, "", "");
        dialog.AddResponse("ok", "Ok");
        dialog.SetResponseAppearance("ok", Adw.ResponseAppearance.Default);
        dialog.SetCloseResponse("ok");

        // TODO: Handle more error codes
        switch (error) {
            case 0:
                dialog.SetHeading(sucessMessage);
                break;
            case 19:
                dialog.SetHeading("Duplicado");
                dialog.SetBody("Ya existe un elemento con ese nombre y ese código");
                break;
            default:
                dialog.SetHeading("Error");
                break;
        }
        return dialog;
    }
}