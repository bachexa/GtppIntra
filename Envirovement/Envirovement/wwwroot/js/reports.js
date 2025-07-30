//alert("Bacho");
var GetJtelReports = $("#EnvirovementReports")[0].innerText;
///JTELStat/GetLiveStat



function GetEnvirovementRepExcelExport() {
    var period = $("#periodExcel option:selected").val();
    var dateOne = document.getElementById("startDateTimeExcel").value;
    var dateTwo = document.getElementById("endDateTimeExcel").value;
    // Construct the URL with query parameters
    var url = "Reports/GetEnvirovementRepExcelExport";
    url += "?period=" + encodeURIComponent(period);
    url += "&dateOne=" + encodeURIComponent(dateOne);
    url += "&dateTwo=" + encodeURIComponent(dateTwo);
    // Create a temporary anchor element
    var link = document.createElement('a');
    link.href = url;
    link.setAttribute('download', 'Report.csv');
    document.body.appendChild(link);
    link.click(); // Trigger the download
    document.body.removeChild(link);
}


    function GetEnvirovementRep() {
        var period = $("#period option:selected").attr("selected", "selected")[0].value;
        var dateOne = document.getElementById("startDateTime").value;
        var dateTwo = document.getElementById("endDateTime").value;

        $.ajax({
            async: true,
            url: "Reports/GetEnvirovementRep",
            traditional: true,
            contentType: "application/json; charset=utf-8",
            data: { period: period, dateOne: dateOne, dateTwo: dateTwo },
            type: "GET",
            dataType: 'JSON',
            success: function (val) {
                var obj = JSON.parse(JSON.stringify(val));
                $("#content").html("");
                if (obj.length == 0) {
                    alert("no data found");
                }
                else {
                    DisplayeEnvRep(obj)
                }
            },
            error: function (req, textStatus, errorThrown) { }
        })
    }
    function DisplayeEnvRep(data)
    {
        $("#EnvirovementRep").hide();
        $("#EnviromentRep").hide();

        // Select the ShowReport div
        var reportDiv = $('#ShowReport');

        // Clear any existing content in the ShowReport div
        reportDiv.empty();

        // Create a new table element
        var table = $('<table>').css({
            'border-collapse': 'separate',
            'border-spacing': '10px',
            'font-size': '18px'
        });

        // Add the header row
        var headerRow = $('<tr>').css('border', '1px solid black');
        headerRow.append('<td style="border: 1px solid black; padding: 20px; color: dodgerblue; font-weight: bold;">TurbinaName</td>');
        headerRow.append('<td style="border: 1px solid black; padding: 20px; color: dodgerblue; font-weight: bold;">NoxValue</td>');
        headerRow.append('<td style="border: 1px solid black; padding: 20px; color: dodgerblue; font-weight: bold;">CoValue</td>');
        headerRow.append('<td style="border: 1px solid black; padding: 20px; color: dodgerblue; font-weight: bold;">RecordDate</td>');
        table.append(headerRow);

        // Loop through the data and append rows to the table
        for (var i = 0; i < data.length; i++) {
            var row = $('<tr>').css('border', '1px solid black');
            row.append('<td style="border: 1px solid black; padding: 20px; ">' + data[i].turbinaName + '</td>');
            if (data[i].noxValue > 25) {
                row.append('<td style="border: 1px solid black; padding: color:red;font-weight:bold">' + data[i].noxValue + '</td>');
            }
            else {
                row.append('<td style="border: 1px solid black; padding: 20px;color:green;font-weight:bold">' + data[i].noxValue + '</td>');
            }
            if (data[i].coValue > 20) {
                row.append('<td style="border: 1px solid black; padding: 20px;color:red;font-weight:bold">' + data[i].coValue + '</td>');
            }
            else {
                row.append('<td style="border: 1px solid black; padding: 20px;color:green;font-weight:bold">' + data[i].coValue + '</td>');
            }
            row.append('<td style="border: 1px solid black; padding: 20px;">' + data[i].recordDate + '</td>');
            table.append(row);
        }

        // Append the newly created table to the ShowReport div
        reportDiv.append(table)
    }


