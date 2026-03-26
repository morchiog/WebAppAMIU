<%@ Page EnableEventValidation="true" Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="LogConservazione.aspx.cs" Inherits="WebAppAMIU.LogConsEcos.LogConservazione" %>

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
    </style>
    <h1>Log Conservazione Sostitutiva</h1>
    <div class="form-horizontal">
        <input type="text" runat="server" id="input_error" style="display: none" />
        <label class="control-label control-label-left" runat="server" labelfor="data_richiesta_da" id="Label1">Data Inserimento da</label>
        <input runat="server" class="data btn white" type="date" id="data_richiesta_da" />
        <label class="control-label control-label-left" runat="server" labelfor="data_richiesta_a" id="Label2">&nbsp;a</label>
        <input runat="server" class="data btn white" type="date" id="data_richiesta_a" />
        <label class="control-label control-label-left" runat="server" labelfor="data_richiesta_a" id="Label4">&nbsp;Registro</label>
        <asp:DropDownList runat="server" ID="ddlReg"></asp:DropDownList>
        <label class="control-label control-label-left" runat="server" labelfor="txtRicerca" id="Label3">Ricerca:</label>
        <input id="txtRicerca" runat="server" type="text" value="" />
        <asp:Button runat="server" ID="BtnCerca" CssClass="button btn" Text="Cerca" TabIndex="1" OnClick="BtnCerca_Click" />
        <asp:Button runat="server" ID="BtnReset" CssClass="button btn" Text="Reset Filtri" OnClick="BtnReset_Click" />

            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;----->>>>>
        <span class="control-label control-label-left">Righe:</span>&nbsp;<asp:Label runat="server" class="control-label control-label-left" ID="lblRighe" ToolTip="Tot." Text="0"></asp:Label>
    </div>
    <h1>
        <label runat="server" id="no_data_lbl" style="display: none; margin-top: 1%;">Non sono presenti dati da mostrare</label></h1>
    <asp:GridView runat="server" CssClass="mt-3 my-gridview-class" ID="data_gridview" AutoGenerateColumns="false" HeaderStyle-ForeColor="White" HeaderStyle-BackColor="#086424" HeaderStyle-HorizontalAlign="Center"
        HeaderStyle-VerticalAlign="Middle" Font-Names="Poppins" Font-Size="16px" ForeColor="Black" RowStyle-HorizontalAlign="Center" BorderColor="Black" BorderWidth="2">
        <Columns>
            <asp:BoundField DataField="tipo" HeaderText="Tipo File" />
            <asp:BoundField DataField="data_ins" HeaderText="Data Invio Cons." DataFormatString="{0:d}" />
            <asp:BoundField DataField="nomef" HeaderText="Nome File" />
            <asp:BoundField DataField="anno" HeaderText="Anno" />
            <asp:BoundField DataField="progda" HeaderText="Prog. Da" />
            <asp:BoundField DataField="proga" HeaderText="Prog. A" />
        </Columns>

    </asp:GridView>
</asp:Content>
