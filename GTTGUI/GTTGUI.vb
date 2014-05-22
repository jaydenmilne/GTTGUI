Public Class GTTGUI
    Dim BackImage As Image
    Dim Fit As Boolean

    Dim Points As New List(Of PointF)
    Dim g As Graphics
    Dim thispen As Pen
    Public CodeText As String

    Dim IsEditingPoint As Boolean = False

    Dim ShowVertRuler As Boolean = False


    Dim WindowSize As Integer = 400
    Private Sub StretchImageToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles StretchImageToolStripMenuItem.Click
        BackImage = OpenFile()
        BackImage = New Bitmap(BackImage, New Size(WindowSize, WindowSize))
        Fit = False
        RefreshImageIfStretched(g)

    End Sub

    Function OpenFile() As Image
        OpenFileDialog1.Filter = "Image Files (*.bmp, *.gif, *.jpeg, *.jpg, *.png, *.tiff)|*.bmp;*.gif;*.jpeg;*.jpg;*.png;*.tiff|All Files (*.*)|*.*"
        OpenFileDialog1.FileName = ""
        If OpenFileDialog1.ShowDialog() = Windows.Forms.DialogResult.OK Then
            Dim originalimage As Bitmap = CType(Image.FromFile(OpenFileDialog1.FileName), Image)
            Return originalimage
        Else
            Return New Bitmap(400, 400)
        End If

    End Function

    Private Sub FitImageToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles FitImageToolStripMenuItem.Click
        BackImage = OpenFile()
        Fit = True
        RefreshImageIfStretched(g)
    End Sub

    Public Sub RefreshImageIfStretched(ByVal ItemToDrawTo As Graphics)
        If Fit Then

            ItemToDrawTo.Clear(Color.White)

            'http://stackoverflow.com/questions/1940581/c-sharp-image-resizing-to-different-size-while-preserving-aspect-ratio

            Dim ratiox As Double = WindowSize / BackImage.Width
            Dim ratioy As Double = WindowSize / BackImage.Height

            Dim ratio As Double = If(ratiox < ratioy, ratiox, ratioy)

            Dim newHeight As Integer = Convert.ToInt32(BackImage.Height * ratio)
            Dim newWidth As Integer = Convert.ToInt32(BackImage.Width * ratio)

            Dim posX As Integer = Convert.ToInt32((WindowSize - (BackImage.Width * ratio)) / 2)
            Dim posY As Integer = Convert.ToInt32((WindowSize - (BackImage.Height * ratio)) / 2)



            ItemToDrawTo.DrawImage(BackImage, posX, posY, newWidth, newHeight)
        Else
            ItemToDrawTo.Clear(Color.White)
            ItemToDrawTo.DrawImage(BackImage, New Point(0, 0))
        End If





    End Sub

    Private Sub AboutGTTGUIToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles AboutGTTGUIToolStripMenuItem.Click
        MessageBox.Show("Made by Jayden Milne 2014", "About GTT GUI")
    End Sub

    Private Sub LicenseToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles LicenseToolStripMenuItem.Click
        MessageBox.Show("http://www.apache.org/licenses/LICENSE-2.0.html" & vbNewLine & "You may not redistribute this under any other license, including the GPL", "License")
    End Sub

    Private Sub ClearImageToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles ClearImageToolStripMenuItem.Click
        BackImage = New Bitmap(200, 200)
        RefreshImageIfStretched(g)
    End Sub

    Private Sub PictureBox_MouseClick(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles PictureBox.MouseClick
        Points.Add(New PointF(e.Location.X / WindowSize, e.Location.Y / WindowSize))

        If SymOverVert Then
            Points.Insert(0, New PointF((WindowSize - e.Location.X) / WindowSize, e.Location.Y / WindowSize))
        End If

        If SymOverHoriz Then
            Points.Insert(0, New PointF(e.Location.X / WindowSize, (WindowSize - e.Location.Y) / WindowSize))
        End If

        RefreshListBox()
        ReDrawLines(g)


    End Sub

    Private Sub ReDrawLines(ByVal ItemToDrawTo As Graphics)

        If Not IsNothing(BackImage) Then
            RefreshImageIfStretched(ItemToDrawTo)
        Else
            ItemToDrawTo.Clear(Color.White)
        End If


        Dim TempList As New List(Of PointF)
        For Each Item In Points
            TempList.Add(New PointF(Item.X * WindowSize, Item.Y * WindowSize))
        Next
        If TempList.Count > 1 Then
            If CheckBox1.Checked Then
                ItemToDrawTo.FillPolygon(thispen.Brush, TempList.ToArray())
            Else
                ItemToDrawTo.DrawLines(thispen, TempList.ToArray())
            End If

        End If


        Dim LinePen As New Pen(Brushes.Red, 1)

        LinePen.DashStyle = Drawing2D.DashStyle.Dash

        If ShowVertRulers Then

            ItemToDrawTo.DrawLine(LinePen, New Point(CInt(WindowSize / 2), 0), New Point(CInt(WindowSize / 2), WindowSize))

        End If

        If SHowHorizRuler Then

            ItemToDrawTo.DrawLine(LinePen, New Point(0, CInt(WindowSize / 2)), New Point(WindowSize, CInt(WindowSize / 2)))

        End If

        If SymOverVert Then
            Dim TransColor = Color.FromArgb(255, Color.Gray)
            Dim Pen As New Pen(TransColor)
            ItemToDrawTo.DrawRectangle(Pen, New Rectangle(New Point(0, 0), New Size(WindowSize, WindowSize)))
        End If



    End Sub

    Private Sub PictureBox_MouseMove(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles PictureBox.MouseMove
        ToolStripStatusLabel2.Text = "X: " & CStr(e.Location.X / WindowSize) & " Y: " & CStr(e.Location.Y / WindowSize)
    End Sub

    Private Sub RefreshListBox()
        ListBox.Items.Clear()
        For Each Item In Points
            ListBox.Items.Add(Item.ToString)
        Next
        Label4.Text = Points.Count.ToString
    End Sub

    Private Sub GTTGUI_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        g = PictureBox.CreateGraphics
        thispen = New Pen(Brushes.Black)
        thispen.Width = 2
        g.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
    End Sub

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click

        Try
            Points.Add(New PointF(CSng(TextBox1.Text), CSng(TextBox2.Text)))
        Catch ex As Exception
            MessageBox.Show("There was an error in your fields. Insert another quarter and try again")
        End Try

        RefreshListBox()
        ReDrawLines(g)
        TextBox1.Text = ""
        TextBox2.Text = ""

    End Sub


    Private Sub GTTGUI_Paint(sender As Object, e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint

    End Sub


    Private Sub WhiteToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles WhiteToolStripMenuItem.Click
        thispen = New Pen(Brushes.White)
        thispen.Width = 2
    End Sub

    Private Sub BlackToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles BlackToolStripMenuItem.Click
        thispen = New Pen(Brushes.Black)
        thispen.Width = 2
    End Sub

    Private Sub Button2_Click(sender As System.Object, e As System.EventArgs) Handles Button2.Click
        ReDrawLines(g)
    End Sub

    Private Sub ShowCodeToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles ShowCodeToolStripMenuItem.Click
        Dim CodeString As String = "Dim UnsizedGeom() as PointF ={"
        For Each Item In Points
            CodeString = CodeString & vbNewLine & vbTab & "New PointF(" & Item.X.ToString & ", " & Item.Y.ToString & "),"
        Next

        CodeString = CodeString & "}"
        CodeText = CodeString

        TextShower.Show()

    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        If ListBox.SelectedIndex >= 0 Then



            TextBox1.Text = Points(ListBox.SelectedIndex).X
            TextBox2.Text = Points(ListBox.SelectedIndex).Y

            IsEditingPoint = True
            Button4.Enabled = True
            Button3.Enabled = False



        Else
            MessageBox.Show("Select something, idiot")
        End If

    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        If IsEditingPoint Then
            Button4.Enabled = False
            Button3.Enabled = True

            Try
                Points(ListBox.SelectedIndex) = New PointF(CSng(TextBox1.Text), CSng(TextBox2.Text))
            Catch ex As Exception
                MessageBox.Show("Please double check your points, something exploded")
            End Try



            IsEditingPoint = False
            RefreshListBox()
            ReDrawLines(g)

        Else
            MessageBox.Show("You're a hacker, hacker.")
        End If
    End Sub

    Private Sub OnToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OnToolStripMenuItem.Click
        g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
        ReDrawLines(g)
    End Sub

    Private Sub OffToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OffToolStripMenuItem.Click
        g.SmoothingMode = Drawing2D.SmoothingMode.HighSpeed
        ReDrawLines(g)
    End Sub

    Dim ShowVertRulers As Boolean = False

    Private Sub HalfwayToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles HalfwayToolStripMenuItem.Click
        SHowHorizRuler = True
    End Sub

    Private Sub OffToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OffToolStripMenuItem1.Click
        ShowVertRulers = False
        SHowHorizRuler = False
    End Sub

    Private Sub ExportImageToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExportImageToolStripMenuItem.Click
        SaveFileDialog1.Filter = "Bitmap (*.bmp)|*.bmp|All Files (*.*)|*.*"
        SaveFileDialog1.FileName = ""
        If SaveFileDialog1.ShowDialog() = Windows.Forms.DialogResult.OK Then
            Dim Bitmap1 As New Bitmap(WindowSize, WindowSize)
            Dim BitGraphics As Graphics = Graphics.FromImage(Bitmap1)
            ReDrawLines(BitGraphics)

            Bitmap1.Save(SaveFileDialog1.FileName)


        End If

    End Sub

    Private Sub OnToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        ShowVertRuler = True
    End Sub

    Private Sub OffToolStripMenuItem2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        ShowVertRuler = False
    End Sub

    Private SymOverVert As Boolean = False
    Dim SymOverHoriz As Boolean = False
    Dim SHowHorizRuler As Boolean = False

    Private Sub ThisWillBeSymetricalOverVerticalAxisToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles ThisWillBeSymetricalOverVerticalAxisToolStripMenuItem.Click
        If Points.Count = 0 Then

            SymOverVert = True

            ShowVertRulers = True

            If SymOverHoriz Then
                Label5.Text = "Symmetry: Vertical and Horizontal"
            Else
                Label5.Text = "Symmetry: Vertical Center"
            End If


        Else
            MessageBox.Show("You must do this with a new design.")
        End If
    End Sub

    Private Sub HorizonalSymmetryToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles HorizonalSymmetryToolStripMenuItem.Click
        If Points.Count = 0 Then

            SymOverHoriz = True

            SHowHorizRuler = True

            If SymOverVert Then

            Else
                Label5.Text = "Symmetry: Horizontal"
            End If

        End If
    End Sub

    Private Sub ListBox_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ListBox.SelectedIndexChanged

        If ListBox.SelectedIndex >= 0 Then
            ReDrawLines(g)

            Dim CircleLocation As New PointF(Points(ListBox.SelectedIndex).X * WindowSize - 4, Points(ListBox.SelectedIndex).Y * WindowSize - 4)

            g.DrawEllipse(Pens.Red, New RectangleF(CircleLocation, New Size(2 * 4, 2 * 4)))
        End If



    End Sub

    Private Sub Button5_Click(sender As System.Object, e As System.EventArgs) Handles Button5.Click
        ListBox.SelectedIndex = -1
    End Sub

    Private Sub VerticalToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles VerticalToolStripMenuItem.Click
        ShowVertRuler = True
    End Sub


    Private Sub CustonToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles CustonToolStripMenuItem.Click
        Dim customcolorname = InputBox("Insert HTML color")

        If Not customcolorname.Contains("#") Then
            customcolorname = customcolorname.Insert(0, "#")
        End If

        Dim width = CInt(InputBox("Width"))

        Dim RealCustomColor = System.Drawing.ColorTranslator.FromHtml(customcolorname)

        thispen = New Pen(RealCustomColor, width)

    End Sub

    Private Sub NewToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles NewToolStripMenuItem.Click
        Points = New List(Of PointF)
        ListBox.Items.Clear()
        BackImage = New Bitmap(WindowSize, WindowSize)
        ListBox.ClearSelected()
        CheckBox1.Checked = False
    End Sub
End Class
