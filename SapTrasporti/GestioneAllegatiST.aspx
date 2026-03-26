<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/PopUp.Master" CodeBehind="GestioneAllegatiST.aspx.cs" Inherits="WebAppAMIU.SapTrasporti.GestioneAllegatiST" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <!-- CSS -->
    <style>
        .content {
            padding-top: 50px;
        }

        .hide {
            display: none !important;
            width: 0px;
        }
        /*select e bottoni*/
        .select {
            background-color: #086424;
            color: white;
        }


        .data /*temporale*/ {
            background-color: #086424;
            color: white;
        }
        /*filtri*/

        #MainContent_indicatore_select {
            display: none;
        }

        #MainContent_btnPopulateIndSelect {
            display: none;
        }

        h1 {
            text-align: center;
        }

        .hiddencol {
            display: none;
        }
    </style>
    <asp:HiddenField runat="server" ID="hdId" />
    <asp:HiddenField runat="server" ID="hdOpeAdminSiNo" />

    <asp:HiddenField runat="server" ID="hdOperatore" />

    <div runat="server" id="divBonif" visible="false">
        <select runat="server" class="select btn" id="ope_bonifica">
            <option value="">--</option>
            <option value="ALBA">Alba Alessia</option>
            <option value="FOLLI">Folli Barbara</option>
            <option value="PALADINO">Paladino Marcella</option>
            <option value="MORCHIOG">Morchio Giuseppe</option>
        </select>
    </div>

    <h1>Gestione allegati agli Allineamenti Anagrafici
        <asp:Label ID="lblPlan" runat="server"></asp:Label>
    </h1>
    <div class="container-fluid">
        <div class="row">
            <div class="col-lg-12" runat="server" id="divLoadDoc">
                <asp:FileUpload Style="margin-top: 4px; color: White; background-color: #086424;" ID="UplDoc" runat="server" accept=".pdf" />&nbsp;&nbsp;<asp:Button Style="margin-top: 4px; color: White; background-color: #086424;" ID="BtnUplDoc" Text="Carica Allegato" runat="server" CommandArgument="0" OnClick="BtnFileUpl_Click" />
            </div>
        </div>
    </div>
    <!-- tabella dati -->
    <asp:Label runat="server" ID="lblHelpDescr" Style="font-size: 20px"></asp:Label>
    <asp:GridView runat="server" CssClass="mt-3 my-gridview-class" ID="data_gridview" AutoGenerateColumns="false" HeaderStyle-ForeColor="White" HeaderStyle-BackColor="#086424" HeaderStyle-HorizontalAlign="Center"
        HeaderStyle-VerticalAlign="Middle" ForeColor="Black" RowStyle-HorizontalAlign="Center" BorderColor="Black" BorderWidth="2" CellSpacing="5" CellPadding="5"
        OnRowCommand="Data_gridview_RowCommand" OnRowDataBound="Data_gridview_RowDataBound">
        <Columns>

            <asp:BoundField DataField="id" HeaderText="Id Verifica" ItemStyle-CssClass="hiddencol" HeaderStyle-CssClass="hiddencol" />
            <asp:BoundField ItemStyle-HorizontalAlign="Left" DataField="percorso_file" HeaderText="Cartella" />
            <asp:BoundField ItemStyle-HorizontalAlign="Left" DataField="nome_file" HeaderText="Nome del File" />
            <asp:BoundField ItemStyle-HorizontalAlign="Left" DataField="tipodescr" HeaderText="" ItemStyle-CssClass="hide" HeaderStyle-CssClass="hide" />
            <asp:BoundField ItemStyle-Width="0" HeaderStyle-Width="0" DataField="id" ItemStyle-CssClass="hiddencol" HeaderStyle-CssClass="hiddencol" />
            <asp:TemplateField HeaderText="Azioni" ItemStyle-Wrap="false">
                <ItemTemplate>
                    <asp:ImageButton runat="server" ID="imgBtnDwldPl" Width="32" ToolTip="scarica documento" ImageUrl="~/Images/dwld_doc.png" CommandArgument='<%# Eval("percorso_file") + ";" + Eval("nome_file") + ";" + Eval("estensione") + ";" + Eval("tipo") %>' CommandName="download" />
                    <asp:ImageButton runat="server" ID="imgDelete" Width="32" ToolTip="cancella documento " OnClientClick="return confirm('Sicuri di voler cancellare questo documento?');" ImageUrl="~/Images/delete_doc.png" CommandArgument='<%# Eval("id") %>' CommandName="cancella" />
                </ItemTemplate>
            </asp:TemplateField>

        </Columns>

    </asp:GridView>
    <!-- decidiamo come comportarci su discorso gestori/bonificatori ?
            <select runat="server" style="display:none" class="select btn" id="ope_gestore">
            <option value="">--</option>
            <option value="MAGGIO">Maggio Andrea</option>
            <option value="STERLINI">Sterlini Alessio</option>
            <option value="MORCHIOG">Morchio G.</option>
        </select>
    -->

    <hr />
    <div class="container-fluid">
        <div class="row" style="align-content: end;">
            <div class="col-lg-5">&nbsp;</div>
            <div class="col-lg-2">
                <!-- tolto se non si trova un modo x evitare che faccia la post invece delal singola reload! window.opener.location.reload(true); -->
                <input id="btnClose" style="font-size: 18px" type="button" value="Close" onclick="window.opener.location.reload(true);window.close();" />
                &nbsp;
            </div>
            <div class="col-lg-5">&nbsp;</div>

        </div>
    </div>

    <script>


        function Controlli() {
           <%-- if ($('#<%=anno_select.ClientID%>').val() === "0") {
                alert('Devi selezionare un anno!');
            }--%>
        }

    </script>
</asp:Content>
