// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {

    // By: Ing. Juan Pablo Valdez Reyes
    function validarCedula(pCedula) {
        let vnTotal = 0;
        let vcCedula = pCedula.replace("-", "");
        let pLongCed = vcCedula.trim().length;
        let digitoMult = [1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1];

        if (pLongCed < 11 || pLongCed > 11)
            return false;

        for (let vDig = 1; vDig <= pLongCed; vDig++) {
            let vCalculo = parseInt(vcCedula.substring(vDig - 1, vDig)) * digitoMult[vDig - 1];
            if (vCalculo < 10)
                vnTotal += vCalculo;
            else
                vnTotal += parseInt(vCalculo.toString().substring(0, 1)) + parseInt(vCalculo.toString().substring(1, 2));
        }

        if (vnTotal % 10 === 0)
            return true;
        else
            return false;
    }

    $(".cedula").on("blur", function () {
        let cedula = $(this).val();
        if (!validarCedula(cedula)) {
            $(this).val("");
            alert("El campo de cedula no es valido...");
        }
    })

    // Funciones para Modulo de Facturacion //

    // Calcular el total al pintar la fila
    function calcularRowTotal(row) {
        var cantidad = parseFloat(row.find(".cantidad").val());
        var precioUnitario = parseFloat(row.find(".precio").val());
        var total = cantidad * precioUnitario;
        row.find(".row_total").text(total.toFixed(2));
    }

    function calcularTotalGeneral() {
        let subTotal = 0;
        let totalGeneral = 0;
        $("#articulosTable > tbody > tr").each(function (index, tr) {
            let totalProducto = parseFloat($(tr).find("td").eq(3).find("span").html());
            subTotal += totalProducto;
        });
        console.log("SubTotal General: " + subTotal);
        $("#Subtotal").val(subTotal);
        $("#invoice_gnr_subtotal").html("$" + subTotal.toFixed(2));

        let itbis = subTotal * 0.18;
        totalGeneral = subTotal + itbis;

        console.log("Total General: " + totalGeneral);
        $("#Total").val(totalGeneral);
        $("#invoice_gnr_total").html("$" + totalGeneral.toFixed(2));

    }

    // Agregar fila
    $("#agregarRowBtn").click(function () {
        var table = $("#articulosTable");
        var newRow = $("<tr>");

        $.ajax({
            url: "/Articulos/GetAllArticulos",
            type: "GET"
        }).done(response => {
            let articulos = response;

            console.log(articulos);

            articulos.forEach((articulo) => {
                console.log(articulo);
            });
            if (articulos !== null && articulos !== undefined && articulos !== 'undefined') {
                if (articulos.length > 0) {

                    let currentRowsCount = table.children("tbody").children("tr").length;

                    let selectArticulos = "<td><select name='FacturaArticulos[" + currentRowsCount + "].IdArticulo' class='articulo form-control'><option value=''>Seleccione un artículo</option>";

                    articulos.forEach(articulo => {
                        let optionElement = `
                            <option value='${articulo.id}' data-precio='${articulo.precio}' >${articulo.descripcion}</option>
                        `;
                        selectArticulos += optionElement;
                    });

                    selectArticulos += "</select></td>";

                    newRow.append(selectArticulos);
                    newRow.append("<td><input type='text' name='FacturaArticulos[" + currentRowsCount + "].Cantidad' class='cantidad form-control' value='1' readonly /></td>");
                    newRow.append("<td><input type='text' name='FacturaArticulos[" + currentRowsCount + "].PrecioUnitario' class='precio form-control-plaintext' readonly /></td>");
                    newRow.append("<td><span class='row_total'></span></td>");
                    newRow.append("<td><button type='button' class='eliminarBtn'>Eliminar</button></td>");

                    table.append(newRow);

                    // Mostrar el precio del artículo seleccionado en la columna "PrecioUnitario"
                    $(".articulo").change(function () {
                        var selectedOption = $(this).find(":selected");
                        var precioUnitario = selectedOption.data("precio");

                        $(this).closest("tr").find(".cantidad").prop("readonly", false);
                        $(this).closest("tr").find(".precio").val(precioUnitario);

                        // Calcular el total al pintar la fila
                        calcularRowTotal($(this).closest("tr"));
                    });
                }
            }
        });
    });

    // Eliminar fila
    $(document).on("click", ".eliminarBtn", function () {
        $(this).closest("tr").remove();
    });

    // Calcular el total al cambiar la cantidad
    $(document).on("change", ".cantidad", function () {
        calcularRowTotal($(this).closest("tr"));

        setTimeout(function () {
            calcularTotalGeneral();
        }, 500)
    });

    // Fin Modulo Facturacion //

    $(document).on("change", ".articulo", function () {
        setTimeout(function () {
            calcularTotalGeneral();
        }, 500)
    });

    // Clase para solo permitir letras en un input
    $(".solo-letras").on("input", function () {
        var value = $(this).val();
        var newValue = value.replace(/[^a-zA-Z]/g, '');
        $(this).val(newValue);
    });
});