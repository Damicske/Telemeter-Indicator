Imports System.Globalization
Imports System.ComponentModel
Imports System.Resources
Imports System.Threading

Module modLocalize
    Private Sub ApplyLocale(ByVal FormTodo As Form, ByVal locale_name As String)
        ' Make a CultureInfo and ComponentResourceManager.
        Dim culture_info As New CultureInfo(locale_name)
        Dim component_resource_manager As New ComponentResourceManager(Me.GetType)

        ' Make the thread use this locale. This doesn't change
        ' existing controls but will apply to those loaded later
        ' and to messages we get for Help About (see below).
        Thread.CurrentThread.CurrentUICulture = culture_info
        Thread.CurrentThread.CurrentCulture = culture_info

        ' Apply the locale to the form itself.
        ' Debug.WriteLine("$this")
        component_resource_manager.ApplyResources(FormTodo, "$this", culture_info)

        ' Apply the locale to the form's controls.
        For Each ctl As Control In FormTodo.Controls
            ApplyLocaleToControl(ctl, component_resource_manager, culture_info)
        Next ctl

        ' Perform manual localizations.
        ' These resources are stored in the Form1 resource
        ' files.
        Dim resource_manager As New ResourceManager("Localized.Form1", FormTodo.GetType.Assembly)

        If FormTodo.components IsNot Nothing Then FormTodo.components.Dispose()
        FormTodo.Controls.Clear()
        FormTodo.InitializeComponent()

    End Sub

    Private Sub ApplyLocaleToControl(ByVal ctl As Control, ByVal component_resource_manager As ComponentResourceManager, ByVal culture_info As CultureInfo)
        ' Debug.WriteLine(ctl.Name)
        component_resource_manager.ApplyResources(ctl, ctl.Name, culture_info)

        ' See what kind of control this is.
        If TypeOf ctl Is MenuStrip Then
            ' Apply the new locale to the MenuStrip's items.
            Dim menu_strip As MenuStrip = DirectCast(ctl, MenuStrip)
            For Each child As ToolStripMenuItem In menu_strip.Items
                ApplyLocaleToToolStripItem(child, component_resource_manager, culture_info)
            Next child
        Else
            ' Apply the new locale to the control's children.
            For Each child As Control In ctl.Controls
                ApplyLocaleToControl(child, component_resource_manager, culture_info)
            Next child
        End If
    End Sub
    ' Recursively apply the locale to a ToolStripItem.
    Private Sub ApplyLocaleToToolStripItem(ByVal item As  _
        ToolStripItem, ByVal component_resource_manager As  _
        ComponentResourceManager, ByVal culture_info As  _
        CultureInfo)
        ' Debug.WriteLine(menu_item.Name)
        component_resource_manager.ApplyResources(item, _
            item.Name, culture_info)

        ' Apply the new locale to items contained in it.
        If TypeOf item Is ToolStripMenuItem Then
            Dim menu_item As ToolStripMenuItem = _
                DirectCast(item, ToolStripMenuItem)
            For Each child As ToolStripItem In _
                menu_item.DropDownItems
                ApplyLocaleToToolStripItem(child, _
                    component_resource_manager, culture_info)
            Next child
        End If
    End Sub
End Module
