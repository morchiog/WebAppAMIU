<%@ Page EnableEventValidation="true" Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="ManSmartCheckIBAN.aspx.cs" Inherits="WebAppAMIU.SmartCheckIBAN.ManSmartCheckIBAN" %>

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
    <h1>Smart Check IBAN </h1>
    <div class="form-horizontal">
        <asp:Button runat="server" ID="btnLogin" Text="Login" CssClass="bottone btn" OnClick="btnLogin_Click" />

        <asp:TextBox runat="server" ID="txtAuxRes"   TextMode="MultiLine" Rows="3" Columns="40" style="resize: both"></asp:TextBox>
    </div>
  
    <div class="form-horizontal">
        <div class="form-inline">
            <asp:UpdatePanel runat="server" ID="UpdFornitore">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btnTrigForn" />
                    <asp:AsyncPostBackTrigger ControlID="txt_fornitore" />
                </Triggers>
                <ContentTemplate>
                    <fieldset>
                        <legend>Ricerca Fornitore</legend>

                        <table border="2">
                            <tr>
                                <td>
                                      <div>
                                          C.FISC.<asp:Label runat="server" ID="lblCfisc"></asp:Label>
                                          <br />
                                          PIVA<asp:Label runat="server" ID="lblPIVA"></asp:Label>
                                          <br />
                                          IBAN<asp:Label runat="server" ID="lblIban"></asp:Label>
                                      </div>
                                </td>
                            </tr>
                            <tr>
                                <td style="vertical-align: text-top">
                                    <asp:TextBox runat="server" ID="txt_fornitore" AutoPostBack="true" Style="width: 180px" OnTextChanged="btnTrigForn_Click"></asp:TextBox>
                                     <asp:ImageButton  Width="20px" ImageUrl="~/Images/littleGreenLens.png" runat="server" ID="btnTrigForn" OnClick="btnTrigForn_Click" />
                                    <asp:Label runat="server" ID="lblError" ForeColor="Red" visible="false"></asp:Label>
                                </td>
                                <td>
                                   
                                    <asp:UpdateProgress ID="UpdateProgress1" runat="Server" AssociatedUpdatePanelID="UpdFornitore">
                                        <ProgressTemplate>

                                            <img id="ImgLoading" width="80" runat="server" src="../Images/loading.gif" style="position: relative; top: 0px; left: 350px" />
                                        </ProgressTemplate>
                                    </asp:UpdateProgress>

                                </td>
                                <td>
                                    <div style="width: 1230px; height: 200px; overflow-y: auto; border: solid; border-width: 0px">
                                        <asp:DataGrid HeaderStyle-Font-Size="15px" HeaderStyle-Font-Bold="true" HeaderStyle-Height="14px" ItemStyle-Height="14px" HeaderStyle-Font-Names="verdana" ItemStyle-Font-Names="verdana"
                                             ItemStyle-Font-Size="14px"   runat="server" GridLines="Horizontal" ID="grdfornitore" AutoGenerateColumns="false" OnItemCommand="grdfornitore_ItemCommand">
                                            <Columns>
                                                <asp:TemplateColumn>
                                                    <ItemTemplate>
                                                        <asp:ImageButton ImageUrl="~/images/leftArrow.png" Width="16px" runat="server" ID="btnSel" CommandName="sel" CommandArgument='<%# Eval("Codice_Fornitore") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateColumn>
                                                <asp:BoundColumn ItemStyle-Font-Size="13px" DataField="Nome_Fornitore" HeaderText="Nome"></asp:BoundColumn>
                                                <asp:BoundColumn ItemStyle-Font-Size="13px" DataField="CFISC" HeaderText="CFISC"></asp:BoundColumn>
                                                <asp:BoundColumn ItemStyle-Font-Size="13px" DataField="PIVA" HeaderText="PIVA"></asp:BoundColumn>
                                                <asp:BoundColumn ItemStyle-Font-Size="13px" DataField="NOME_BANCA" HeaderText="BANCA"></asp:BoundColumn>
                                                <asp:BoundColumn ItemStyle-Font-Size="13px" DataField="IBAN" HeaderText="IBAN"></asp:BoundColumn>
                                            </Columns>
                                        </asp:DataGrid>
                                    </div>

                                </td>
                            </tr>
                        </table>
                    </fieldset>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>


    <script>




</script>


</asp:Content>
