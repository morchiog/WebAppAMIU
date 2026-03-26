<%@ Page EnableEventValidation="true" Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="ListaSapTrasporti.aspx.cs" Inherits="WebAppAMIU.SapTrasporti.ListaSapTrasporti" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .bottone {
            background-color: #086424;
            color: white;
        }

        .btn {
            background-color: #086424;
            color: white;
            font-size: 18px;
            font-family: Poppins;
        }

        .hide {
            display: none !important;
            width: 0px;
        }

        .select {
            background-color: #086424;
            color: white;
            text-align: left;
        }

        .prot_spor {
            white-space: nowrap;
        }

        .testo_filtri {
            font-weight: bold;
            font-size: large;
            /*vertical-align: text-top;*/
        }

        .white {
            color: white;
            font-size: 18px;
            font-family: Poppins;
        }

        .control-label {
            padding-top: 7px;
            margin-bottom: 0;
            text-align: right;
            font: 15pt/0 'Poppins';
        }

        h1 {
            text-align: center;
            font-family: Poppins;
        }

        .content {
            padding-top: 130px;
        }


        /*modal*/
        /* The Modal (background) */
        .modal {
            display: none; /* Hidden by default */
            position: fixed; /* Stay in place */
            z-index: 1; /* Sit on top */
            padding-top: 100px; /* Location of the box */
            left: 0;
            top: 0;
            width: 100%; /* Full width */
            height: 100%; /* Full height */
            overflow: auto; /* Enable scroll if needed */
            background-color: rgb(0,0,0); /* Fallback color */
            background-color: rgba(0,0,0,0.4); /* Black w/ opacity */
        }

        /* Modal Content */
        .modal-content {
            position: relative;
            background-color: #fefefe;
            margin: auto;
            padding: 0;
            border: 1px solid #888;
            width: 700px;
            box-shadow: 0 4px 8px 0 rgba(0,0,0,0.2),0 6px 20px 0 rgba(0,0,0,0.19);
            -webkit-animation-name: animatetop;
            -webkit-animation-duration: 0.4s;
            animation-name: animatetop;
            animation-duration: 0.4s;
        }

        /* Add Animation */
        @-webkit-keyframes animatetop {
            from {
                top: -200px;
                opacity: 0;
            }

            to {
                top: 0;
                opacity: 1;
            }
        }

        @keyframes animatetop {
            from {
                top: -200px;
                opacity: 0;
            }

            to {
                top: 0;
                opacity: 1;
            }
        }

        /* The Close Button */
        .close {
            color: white;
            float: right;
            font-size: 15px;
            font-weight: bold;
        }

            .close:hover,
            .close:focus {
                color: #000;
                text-decoration: none;
                cursor: pointer;
            }

        .modal-header {
            padding: 25px;
            background-color: #5cb85c;
            color: white;
            height: 60px;
        }

        .modal-body {
            padding: 5px;
            height: 330px;
        }

        .modal-footer {
            padding: 2px 16px;
            background-color: #5cb85c;
            color: white;
        }
    </style>
    <h1>Lista "Trasporti" SAP</h1>
    <div class="form-horizontal">
        <input type="text" runat="server" id="input_error" style="display: none" />
        <label class="control-label control-label-left" runat="server" labelfor="data_richiesta_da" id="Label1">Data Inserimento da</label>
        <input runat="server" class="data btn white" type="date" id="data_richiesta_da" />
        <label class="control-label control-label-left" runat="server" labelfor="data_richiesta_a" id="Label2">&nbsp;a</label>
        <input runat="server" class="data btn white" type="date" id="data_richiesta_a" />
        <label class="control-label control-label-left" runat="server" labelfor="txtRicerca" id="Label3">Ricerca:</label>
        <input id="txtRicerca" runat="server" type="text" value="" />
        <asp:Button runat="server" ID="BtnCerca" CssClass="button btn" Text="Cerca" TabIndex="1" OnClick="BtnCerca_Click" />
        <asp:Button runat="server" ID="BtnReset" CssClass="button btn" Text="Reset Filtri" OnClick="BtnReset_Click" />
        <asp:Button runat="server" ID="BtnInsert" CssClass="button btn" Text="Inserimento" OnClientClick="showpopup(this,'I'); return false;" />

    </div>
    <h1>
        <label runat="server" id="no_data_lbl" style="display: none; margin-top: 1%;">Non sono presenti dati da mostrare</label></h1>
    <asp:GridView runat="server" CssClass="mt-3 my-gridview-class" ID="data_gridview" AutoGenerateColumns="false" HeaderStyle-ForeColor="White" HeaderStyle-BackColor="#086424" HeaderStyle-HorizontalAlign="Center"        
        HeaderStyle-VerticalAlign="Middle" Font-Names="Poppins" AlternatingRowStyle-BackColor="Gainsboro" Font-Size="16px" ForeColor="Black" RowStyle-HorizontalAlign="Center" BorderColor="Black" BorderWidth="2"
        OnRowCommand="Data_gridview_RowCommand" OnRowDataBound="Data_gridview_RowDataBound">
        <Columns>
              <asp:TemplateField HeaderText="#" ItemStyle-BorderColor="Gray" ItemStyle-BorderStyle="Solid" ItemStyle-BorderWidth="1">
                  <ItemTemplate>
                      <asp:Label runat="server" ID="lblId" CssClass="lblSelID" Text='<%# Bind("ID") %>'></asp:Label>
                  </ItemTemplate>
              </asp:TemplateField>
            <asp:BoundField DataField="data_ins" HeaderText="Data Invio Cons." DataFormatString="{0:d}" ItemStyle-BorderColor="Gray" ItemStyle-BorderStyle="Solid" ItemStyle-BorderWidth="1"  />
            <asp:BoundField DataField="utente_ins" HeaderText="Utente Ins." ItemStyle-BorderColor="Gray" ItemStyle-BorderStyle="Solid" ItemStyle-BorderWidth="1" />
            <asp:TemplateField HeaderText="N. Trasporto" ItemStyle-BorderColor="Gray" ItemStyle-BorderStyle="Solid" ItemStyle-BorderWidth="1" >
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblNTrasp" CssClass="lblSelNTrasp" Text='<%# Bind("NUM_TRASP") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="N. Ticket" ItemStyle-BorderColor="Gray" ItemStyle-BorderStyle="Solid" ItemStyle-BorderWidth="1">
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblNTicket" CssClass="lblSelNTicket" Text='<%# Bind("TICKET") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Nota" ItemStyle-BorderColor="Gray" ItemStyle-BorderStyle="Solid" ItemStyle-BorderWidth="1">
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblNota" CssClass="lblSelNota" Text='<%# Bind("NOTA") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Azioni" ItemStyle-BorderColor="Gray" ItemStyle-BorderStyle="Solid" ItemStyle-BorderWidth="1">
                <ItemTemplate>
                    <asp:LinkButton CssClass="btn" ID="btnModal" Text="Modifica" OnClientClick="showpopup(this,'U'); return false;" runat="server" BackColor="#086424" ForeColor="White"></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Allegati" ItemStyle-Wrap="false" ItemStyle-BorderColor="Gray" ItemStyle-BorderStyle="Solid" ItemStyle-BorderWidth="1">
                <ItemTemplate>
                    <asp:TextBox ID="txtListaImg" Style="display: none" runat="server" Text='<%# Bind("Lista_Allegati") %>' />
                    <asp:ImageButton runat="server" ID="imgBtnDwldPl" ToolTip="scarica Documento" ImageUrl="~/Images/dwld_doc.png" CommandName="download" CommandArgument='<%# Bind("Lista_Allegati") %>' />
                    <asp:ImageButton runat="server" ID="imgManAllegati" ToolTip="gestisci allegati" ImageUrl="~/Images/man_allegati.png" />
                    <asp:Label ID="lblNumAllegati" runat="server" Style="background-color: transparent; z-index: 1000; font-size: 22px; font-family: poppins; font-weight: bold; margin-left: -20px;" Text="0"></asp:Label>
                    <asp:Label ID="Label10" runat="server" ForeColor="Transparent" Text="-"></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>

    </asp:GridView>

    <asp:Button ID="btnHiddenSave" runat="server" Style="display: none" Text="to be hidden" OnClick="btnHiddenSave_Click" />
            <asp:HiddenField runat="server" ID="hdMode" />
            <asp:HiddenField runat="server" ID="hdId" />

    <div id="myModal" class="modal">
        <!-- Modal content -->
        <div class="modal-content">
            <div class="modal-header">
                <span style="margin-left: 180px; font-size: 24px; font-weight:bold">Dati Trasporto</span>
                <span class="close" onclick="closepopup()">X</span>
            </div>
            <div class="modal-body">
                <table>
                    <tr>
                        <td>Nr. trasporto:
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtNumTrasp"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>Ticket:
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtTicket"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>Note:
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtNote" TextMode="MultiLine" Rows="3" Columns="45" Style="resize: none"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="align-content: flex-end">
                            <table style="width: 690px; margin-top: 80px;">
                                <tr>
                                    <td style="width: 250px">&nbsp;</td>
                                    <td>
                                        <button class="button btn" onclick="okpopup('C'); return false;">Salva</button>
                                    </td>
                                    <td>
                                        <button class="button btn" onclick="closepopup(); return false;">Annulla</button>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
    <script>

        var selectedRow = undefined;
        function showpopup(obj, mode) {
            var modal = document.getElementById('myModal');
            modal.style.display = "block";
            selectedRow = obj.parentElement.parentElement;

            
            var hdModeT = document.getElementById('<%=hdMode.ClientID%>');
            hdModeT.innerHTML = mode;
            console.log('step 00' + mode);

            if (mode == 'I') { // update
                document.getElementById('<%=hdMode.ClientID%>').value = 'I';
                document.getElementById('<%=hdId.ClientID%>').value = '-1';                
            }

            if (mode == 'U') { // update
                document.getElementById('<%=hdMode.ClientID%>').value = 'U';
                selectedRow = obj.parentElement.parentElement;
                var lblid = selectedRow.getElementsByClassName('lblSelID')[0];
                var lblNota = selectedRow.getElementsByClassName('lblSelNota')[0];
                var lblTick = selectedRow.getElementsByClassName('lblSelNTicket')[0];
                var lblTras = selectedRow.getElementsByClassName('lblSelNTrasp')[0];
                var valueNota = lblNota.innerHTML;
                var valueTick = lblTick.innerHTML;
                var valueTras = lblTras.innerHTML;
                var valueId = lblid.innerHTML;

                console.log(valueNota + '--' + valueTick + '--' + valueTras);

                document.getElementById('<%=txtTicket.ClientID%>').value = valueTick;
                document.getElementById('<%=txtNumTrasp.ClientID%>').value = valueTras;
                document.getElementById('<%=txtNote.ClientID%>').value = valueNota;
                document.getElementById('<%=hdId.ClientID%>').value = valueId;                
            }
        }
        function closepopup() {
            var modal = document.getElementById('myModal');
            modal.style.display = "none";
        }
        function okpopup(nextState) {
            var modal = document.getElementById('myModal');
            modal.style.display = "none";
            document.getElementById('<%=btnHiddenSave.ClientID%>').click();
        }

        function OpenManAllegati(id, opeSiNo) {
            window.open('GestioneAllegatiST.aspx?id=' + id + "&opeAdmin=" + opeSiNo, 'newwinAllAnag', 'popup=true,height=510px,width=820px,top=200px,left=200px');
        }


    </script>


</asp:Content>
