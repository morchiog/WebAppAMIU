<%@ Page EnableEventValidation="true" Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="ManSmartCheckIBAN.aspx.cs" Inherits="WebAppAMIU.SmartCheckIBAN.ManSmartCheckIBAN" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        label {
            margin-left: 3px;
            margin-right: 10px;
            font-size: 18px;
        }

        .hide {
            display: none !important;
            width: 0px;
        }

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
    </style>
    <h1>Smart Check IBAN </h1>
    <div class="form-horizontal">
        <label class="control-label control-label-left" runat="server" labelfor="txtRicerca" id="Label3">Ricerca:</label>
        <input id="txtRicerca" runat="server" type="text" value="" />

        &nbsp; &nbsp;
        <label runat="server" id="Label1" class="control-label control-label-left">Stato Verifica</label>
        <asp:CheckBoxList RepeatLayout="Flow" runat="server" ID="chkStatoVerif" RepeatDirection="Horizontal" RepeatColumns="6">
            <asp:ListItem Text="Da Verificare" Value="--"></asp:ListItem>
            <asp:ListItem Text="OK" Value="OK"></asp:ListItem>
            <asp:ListItem Text="KO" Value="KO"></asp:ListItem>
        </asp:CheckBoxList>
        &nbsp;
             <asp:Button runat="server" ID="btnSearch" CssClass="button btn" Text="Filtra" OnClick="btnSearch_Click" />
        <asp:Button runat="server" ID="btnReset" CssClass="button btn" Text="Reset Filtri" OnClick="btnReset_Click" />
        <asp:Button runat="server" ID="btnExportExcel" CssClass="button btn" Text="Excel" OnClick="btnExportExcel_Click" />

    </div>


    <div id="divVerif" class="form-horizontal" style="display:none">
        <asp:Button runat="server" ID="btnLogin" Text="Verifica" CssClass="bottone btn" OnClick="btnLogin_Click" />
        <div>
            <asp:TextBox runat="server" style="display:none" ID="txtCodForn"></asp:TextBox>
            Codice Fiscale:<asp:TextBox runat="server" ID="lblCfisc"></asp:TextBox>
            <br />Partita IVA:<asp:TextBox runat="server" ID="lblPIVA"></asp:TextBox>
            <br />IBAN:<asp:TextBox runat="server" ID="lblIban"></asp:TextBox>
            <asp:TextBox runat="server" style="display:none" ID="txtAuxRes"></asp:TextBox>
        </div>
    </div>

    <h1>
        <label runat="server" id="no_data_lbl" style="display: none; margin-top: 1%;">Non sono presenti dati da mostrare</label></h1>
    <asp:GridView runat="server" CssClass="mt-3 my-gridview-class" ID="data_gridview" AutoGenerateColumns="false" HeaderStyle-ForeColor="White" HeaderStyle-BackColor="#086424" HeaderStyle-HorizontalAlign="Center"
        OnRowDataBound="data_gridview_RowDataBound" OnRowCommand="data_gridview_RowCommand" HeaderStyle-VerticalAlign="Middle" ForeColor="Black" RowStyle-HorizontalAlign="Center"
        HeaderStyle-BorderWidth="6" ItemStyle-BorderWidth="6" BorderColor="Transparent">
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:LinkButton runat="server" ID="bntVerifica" Text="Verifica" CssClass="bottone btn" OnClientClick="ApriVerifica(this);return false;"></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="CODICE_FORNITORE" HeaderText="Codice Fornitore" ReadOnly="true" />
            <asp:BoundField DataField="NOME_FORNITORE" HeaderText="Nome Fornitore" ReadOnly="true" ItemStyle-HorizontalAlign="Left" />
            <asp:BoundField DataField="CFISC" HeaderText="Codice Fiscale" ReadOnly="true" ItemStyle-HorizontalAlign="Left" />
            <asp:BoundField DataField="PIVA" HeaderText="Partita IVA" ReadOnly="true" />
            <asp:BoundField DataField="NOME_BANCA" HeaderText="Banca" ReadOnly="true" ItemStyle-HorizontalAlign="Left" />
            <asp:BoundField DataField="IBAN" HeaderText="Iban Fornitore" ReadOnly="true" ItemStyle-HorizontalAlign="Left" />
            <asp:BoundField DataField="data_check" HeaderText="Data Ultimo Controllo " ReadOnly="true" DataFormatString="{0:d}" />
            <asp:BoundField DataField="esito_check" HeaderText="Esito Controllo" ReadOnly="true" />
            <asp:BoundField DataField="note" HeaderText="Note" ReadOnly="true" />
            
        </Columns>
    </asp:GridView>
  


    <script>
        function ApriVerifica(obj) {
            divVerif.style.display = 'block'; 
            selectedRow = obj.parentElement.parentElement;
            cellCodForm = selectedRow.cells[1];
            cellNome = selectedRow.cells[2];
            cellCFISC = selectedRow.cells[3];
            cellPIVA = selectedRow.cells[4];
            cellIBAN = selectedRow.cells[6];
            console.log(cellNome);
            console.log(cellNome.innerHTML);

            varPIVA = cellPIVA.innerHTML;
            if (varPIVA == '&nbsp;') {
                varPIVA = null;
            }
            varCFISC = cellCFISC.innerHTML;
            if (varCFISC == '&nbsp;') {
                varCFISC = null;
            }

            document.getElementById('<%=txtCodForn.ClientID%>').value = cellCodForm.innerHTML;
            document.getElementById('<%=lblCfisc.ClientID%>').value = varCFISC;
            document.getElementById('<%=lblPIVA.ClientID%>').value = varPIVA;
            document.getElementById('<%=lblIban.ClientID%>').value = cellIBAN.innerHTML;

        }



    </script>


</asp:Content>
