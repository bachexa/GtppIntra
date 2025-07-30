

function GetTAReportForHR()
{
    var team = $("#department option:selected").attr("selected", "selected")[0].value;
    var dateOne = document.getElementById("from-date").value;
    var dateTwo = document.getElementById("to-date").value;
    $.ajax({
        async: true,
        url: "TimeAndAtance/GetTARepprt",
        traditional: true,
        contentType: "application/json; charset=utf-8",
        data: { team: team, dateOne: dateOne, dateTwo: dateTwo },
        type: "GET",
        dataType: 'JSON',
        success: function (val) {
            var obj = JSON.parse(JSON.stringify(val));
            $("#content").html("");
            if (obj.length == 0) {
                alert("no data found");
            }
            else {
                DisplayTAReportsForHR(obj)
            }
        },
        error: function (req, textStatus, errorThrown) { }
    })
}


function DisplayTAReportsForHR(data)
{
    
    $("#reporMainPage").hide();
    //alert("bacho"); $('#ShowReport');
    document.getElementById('exportable-table').style.display = 'block';

    var results = $('#exportable-table');
    results.empty();
    var hOne = $('<h1>');
    hOne.append('სამუშაო დროის აღრიცხვის ფორმა')

    var hTwo = $('<h2>');
    hTwo.append('შპს "გარდაბნის თბოსადგური"')

    var h4 = $('<h4>');
    var span = $('<span style="color: #007bff">');
    span.append(data[0].unit);
    h4.append('სტრუქტურული ერთეული: ')
    h4.append(span);

    var h42 = $('<h4>');
    var span2 = $('<span style="color: #007bff">');
    span2.append(' 404428071');
    h42.append('საიდენთიფიკაციო კოდი:  ')
    h42.append(span2);
    results.append(hOne);
    results.append(hTwo);
    results.append(h4);
    results.append(h42);


    // Create the table structure dynamically
    var table = $('<table>').addClass('date-section');

    // Create the first row
    var row1 = $('<tr>');
    var td1 = $('<td>').attr('colspan', '4').attr('rowspan', '3').addClass('left').text('შედგენის თარიღი : ' + data[0].compilationDate);
    var td2 = $('<td>').attr('colspan', '4').addClass('date').text('საანგარიშო პერიოდი');
    row1.append(td1).append(td2);
    table.append(row1);

    // Create the second row
    var row2 = $('<tr>');
    var td3 = $('<td style="background-color: #f2f2f2">').attr('colspan', '2').addClass('center').text('-დან');
    var td4 = $('<td style="background-color: #f2f2f2">').attr('colspan', '2').addClass('center').text('-მდე');
    row2.append(td3).append(td4);
    table.append(row2);

    // Create the third row
    var row3 = $('<tr>');
    var td5 = $('<td>').attr('colspan', '2').addClass('date').text(data[0].datefrom);
    var td6 = $('<td>').attr('colspan', '2').addClass('date').text(data[0].dateTo);
    row3.append(td5).append(td6);
    table.append(row3);

    // Append the table to the results div
    results.append(table);


    // Create the table and append it to the result container
    var table = $('<table>').addClass('table table-bordered'); // You can add more classes like 'table-striped'

    // Create the table header
    var thead = $('<thead>');
    var headerRow1 = $('<tr>');
    headerRow1.append('<th style="background-color: #f2f2f2;" rowspan="2">რიგითი №</th>')
        .append('<th style="background-color: #f2f2f2; " rowspan="2">სახელი გვარი</th>')
        .append('<th style="background-color: #f2f2f2" rowspan="2">თანამდებობა (სპეციალობა)</th>')
        .append('<th style="background-color: #f2f2f2" rowspan="2">ტაბელის ნომერი</th>')
        .append('<th style="background-color: #f2f2f2" colspan="31">' + data[0].unit + '</th>')
        .append('<th style="background-color: #f2f2f2" rowspan="2">სულ ნამუშევარი (საათი)</th>')
        .append('<th style="background-color: #f2f2f2" rowspan="2">სულ ნამუშევარი დღე / ცვლა</th>');

    var headerRow2 = $('<tr>');
    for (var i = 1; i <= data[0].data.length; i++) {
        headerRow2.append('<th>' + i + '</th>');
    }

    // Append the header rows to thead
    thead.append(headerRow1);
    thead.append(headerRow2);
    table.append(thead);

    // Create the table body
    var tbody = $('<tbody>');

    // Data for table rows (replace this with real data if available)

    var employees = [];
    for (var i = 0; i < data.length; i++) {

        employees.push({
            id: i + 1, name: data[i].employee.replace(/^\d+/, ''), position: data[i].position, tabelNumber: data[i].tabelNumber, times: data[i].data, totalHours: data[i].fulltimeStr, totalDays: data[i].workingDayes
        })
    }

    // Create rows for each employee
    for (var j = 0; j < employees.length; j++) {
        var emp = employees[j]; // Access each employee object
        var row = $('<tr>'); // Create a new row for each employee
        // Append employee details to the row
        row.append('<td>' + emp.id + '</td>');
        row.append('<td>' + emp.name + '</td>');
        row.append('<td>' + emp.position + '</td>');
        row.append('<td>' + emp.tabelNumber + '</td>'); // Empty field for "ტაბელის ნომერი"

        //Loop over the 'times' array for each employee and set the background color to yellow
        for (var i = 0; i < emp.times.length; i++) {
            var newTd;

            if (emp.times[i].vacantion === 'vacantion') {
                newTd = $('<td style="background-color: #974706"></td>');
            }
            else if (
                (emp.times[i].inTime === 'Saturday' || emp.times[i].inTime === 'Sunday' || emp.times[i].inTime === 'PublicHolliDay') &&
                (emp.times[i].inTime !== '' || emp.times[i].outTime !== '')
            ) {
                newTd = $('<td style="background-color: #ffff00"></td>');
            }
            else if (
                emp.times[i].inTime === '' && emp.times[i].outTime === '' &&
                emp.times[i].vacantion !== 'vacantion'
            ) {
                newTd = $('<td style="background-color: #ff6961"></td>');
            }
            else if (emp.times[i].inTime !== '' || emp.times[i].outTime !== '') {
                let value = '';
                if (emp.times[i].fullInt > 0) {
                    value = (emp.times[i].fullInt / 60).toFixed(1);
                }

                newTd = $('<td style="background-color: #90ee90; font-size:12px; padding:0; cursor:pointer;">' + value + '</td>');

                // Attach popup click handler
                (function (td, inTime, outTime, date, employeeName) {
                    td.on('click', function () {
                        const width = 200;
                        const height = 200;
                        const left = (screen.width / 2) - (width / 2);
                        const top = (screen.height / 2) - (height / 2);
                        const formattedDate = date.split('T')[0].replace(/-/g, '.');
 

                        const popup = window.open('', '', `width=${width},height=${height},top=${top},left=${left}`);
                                popup.document.write(`
                        <html>
                            <head><title>დრო</title></head>
                            <body style="font-family: Arial; font-size: 14px; padding: 10px;">
                                 <p><strong>თანამშრომელი:</strong> ${employeeName}</p>
                                 <p><strong>თარიღი:</strong> ${formattedDate}</p>
                                <p><strong>მოსვლის დრო:</strong> ${inTime}</p>
                                <p><strong>წასვლის დრო:</strong> ${outTime}</p>
                            </body>
                        </html>
                    `);
                        popup.document.close();
                    });
                })(newTd, emp.times[i].inTime, emp.times[i].outTime, emp.times[i].date, emp.name);
            }
            else {
                newTd = $('<td></td>'); // fallback
            }

            row.append(newTd);

        }

        // Append the total hours and total days columns
        row.append('<td>' + emp.totalHours + '</td>');
        row.append('<td>' + emp.totalDays + '</td>');

        // Append the row to tbody
        tbody.append(row);
    }


    // Append the tbody to the table
    table.append(tbody);

    // Create the table and append it to the result container
    var table1 = $('<table>').addClass('legend-section');

    // First row
    var row1 = $('<tr>');
    row1.append('<td class="left-align">ნამუშევარი</td>');
    row1.append('<td><div class="color-box green"></div> მოსვლა</td>');
    row1.append('<td><div class="color-box red"></div> არმოსვლა</td>');
    row1.append('<td><div class="color-box yellow"></div> უქმე დღე</td>');
    table1.append(row1);

    // Second row
    var row2 = $('<tr>');
    row2.append('<td class="left-align">არგამოცხადების</td>');
    row2.append('<td><div class="color-box blue"></div> თვითიზოლაცია</td>');
    row2.append('<td><div class="color-box gray"></div> დისტანციური</td>');
    row2.append('<td><div class="color-box orange"></div> საავადმყოფო ფურცელი</td>');
    row2.append('<td colspan="2"><div class="color-box yellow-dark"></div> შვებულება</td>');
    table1.append(row2);

    // Third row (signature row)
    var row3 = $('<tr>');
    row3.append('<td style="font-weight: bold;" class="signature-section" colspan="3">ორგანიზაციის/სტრუქტურული ქვედანაყოფის ხელმძღვანელი</td>');
    row3.append('<td style="font-weight: bold; class="signature-section" colspan="3">თანამდებობა</td>');
    row3.append('<td style="font-weight: bold; class="signature-section">ხელმოწერა</td>');
    table1.append(row3);

    // Fourth row (signature details)
    var row4 = $('<tr>');
    row4.append('<td class="signature-section" colspan="3">ავთანდილ შელიავა</td>');
    row4.append('<td class="signature-section" colspan="3">IT სამსახურის უფროსი</td>');
    row4.append('<td class="signature-section"></td>');
    table1.append(row4);


    // Append the complete table to the results div
    results.append(table);
    //results.append(table1);


    var exportButton = $('<button>')
        .attr('id', 'exportToExcelBtn') // Set the id for the button
        .addClass('btn btn-primary') // Add Bootstrap class for styling (or any custom class)
        .text('Export to Excel');

    exportButton.css({
        'padding': '10px 20px',
        'background-color': '#007bff',
        'border': 'none',
        'color': '#fff',
        'cursor': 'pointer',
        'font-size': '16px'
    });

    results.append(exportButton);


    $("#exportToExcelBtn").on('click', function () {
        exportToExcel(data);
    });
}

function GetTAReport() {

    var selectedDepartments = getSelectedDepartments();

    var team = $("#department option:selected").attr("selected", "selected")[0].value;
    var dateOne = document.getElementById("from-date").value;
    var dateTwo = document.getElementById("to-date").value;
    //alert(team);
    $.ajax({
        async: true,
        url: "TimeAndAtance/GetTARepprt",
        traditional: true,
        contentType: "application/json; charset=utf-8",
        data: { team: team, dateOne: dateOne, dateTwo: dateTwo },
        type: "GET",
        dataType: 'JSON',
        success: function (val) {
            var obj = JSON.parse(JSON.stringify(val));
            $("#content").html("");
            if (obj.length == 0) {
                alert("no data found");
            }
            else {
                DisplayTAReports(obj)
            }
        },
        error: function (req, textStatus, errorThrown) { }
    })
}


function getSelectedDepartments() {
    var selectedDepartments = [];

    // Loop through each checked checkbox and get its value
    $('.department-checkbox:checked').each(function () {
        selectedDepartments.push($(this).val());
    });

    return selectedDepartments; // This will return an array of selected department values
}


function GetInOutReport() {
    var selectedDepartments = getSelectedDepartments();
    var team = selectedDepartments;
    //var team = $("#department option:selected").attr("selected", "selected")[0].value;
    var dateOne = document.getElementById("from-date").value;
    var dateTwo = document.getElementById("to-date").value;
    
    $.ajax({
        async: true,
        url: "TimeAndAtance/GetInOutReport",
        traditional: true,
        contentType: "application/json; charset=utf-8",
        data: { team: team, dateOne: dateOne, dateTwo: dateTwo },
        type: "GET",
        dataType: 'JSON',
        success: function (val) {
            var obj = JSON.parse(JSON.stringify(val));
            $("#content").html("");
            if (obj.length == 0) {
                alert("no data found");
            }
            else {
                DisplayInOutReports(obj)
            }
        },
        error: function (req, textStatus, errorThrown) { }
    })
}


function DisplayInOutReports(data) {
    exportInOutExcel(data);
}

function exportInOutExcel(data)
{
    var workbook = new ExcelJS.Workbook();
    var worksheet = workbook.addWorksheet("ცენტრალური აპარატი");

    worksheet.getColumn(1).width = 35;
    worksheet.getRow(2).height = 30;
    worksheet.getColumn(2).width = 35;
    worksheet.getColumn(3).width = 20;
    worksheet.getColumn(4).width = 20;
    worksheet.getColumn(5).width = 20;

    worksheet.mergeCells('A1:E1');
    worksheet.getCell('A1').value = 'სამუშაო დროის აღრიცხვის ფორმა ';
    worksheet.getCell('A1').font = { size: 24, bold: true };
    worksheet.getCell('A1').alignment = { horizontal: 'center', vertical: 'middle' };

    //worksheet.mergeCells('A2:D3');
    worksheet.getCell('A2').value = 'სახელი გვარი ';
    worksheet.getCell('A2').font = { size: 12, bold: true };
    worksheet.getCell('A2').alignment = { horizontal: 'center', vertical: 'middle' };
    worksheet.getCell('A2').fill = {
        type: 'pattern',
        pattern: 'solid',
        fgColor: { argb: 'c6e0b4' } 
    };

    //worksheet.mergeCells('E2:I3');
    worksheet.getCell('B2').value = 'თანამდებობა ';
    worksheet.getCell('B2').font = { size: 12, bold: true };
    worksheet.getCell('B2').alignment = { horizontal: 'center', vertical: 'middle' };
    worksheet.getCell('B2').fill = {
        type: 'pattern',
        pattern: 'solid',
        fgColor: { argb: 'c6e0b4' } 
    };

    //worksheet.mergeCells('J2:K3');
    worksheet.getCell('C2').value = 'მოსვლა ';
    worksheet.getCell('C2').font = { size: 12, bold: true };
    worksheet.getCell('C2').alignment = { horizontal: 'center', vertical: 'middle' };
    worksheet.getCell('C2').fill = {
        type: 'pattern',
        pattern: 'solid',
        fgColor: { argb: 'c6e0b4' } 
    };

    //worksheet.mergeCells('L2:M3');
    worksheet.getCell('D2').value = 'წასვლა ';
    worksheet.getCell('D2').font = { size: 12, bold: true };
    worksheet.getCell('D2').alignment = { horizontal: 'center', vertical: 'middle' };
    worksheet.getCell('D2').fill = {
        type: 'pattern',
        pattern: 'solid',
        fgColor: { argb: 'c6e0b4' } 
    };

    //worksheet.mergeCells('N2:O3');
    worksheet.getCell('E2').value = 'თარიღი ';
    worksheet.getCell('E2').font = { size: 12, bold: true };
    worksheet.getCell('E2').alignment = { horizontal: 'center', vertical: 'middle' };
    worksheet.getCell('E2').fill = {
        type: 'pattern',
        pattern: 'solid',
        fgColor: { argb: 'c6e0b4' } 
    };


    let rowIndex = 3;
    for (var i = 0; i < data.length; i++) {

        worksheet.mergeCells(`A${rowIndex}:B${rowIndex}`);
        worksheet.getCell(`A${rowIndex}`).value = data[i][0].unit; // Access unit for each data[i]
        worksheet.getCell(`A${rowIndex}`).font = { size: 12, bold: true };
        worksheet.getCell(`A${rowIndex}`).alignment = { horizontal: 'center', vertical: 'middle' };

        ////worksheet.mergeCells(`J${rowIndex}:K${rowIndex}`);
        //worksheet.getCell(`B${rowIndex}`).font = { size: 12, bold: true };
        //worksheet.getCell(`B${rowIndex}`).alignment = { horizontal: 'center', vertical: 'middle' };

        //worksheet.mergeCells(`L${rowIndex}:M${rowIndex}`);
        worksheet.getCell(`C${rowIndex}`).font = { size: 12, bold: true };
        worksheet.getCell(`C${rowIndex}`).alignment = { horizontal: 'center', vertical: 'middle' };

        //worksheet.mergeCells(`N${rowIndex}:O${rowIndex}`);
        worksheet.getCell(`D${rowIndex}`).font = { size: 12, bold: true };
        worksheet.getCell(`D${rowIndex}`).alignment = { horizontal: 'center', vertical: 'middle' };

        worksheet.getCell(`E${rowIndex}`).font = { size: 12, bold: true };
        worksheet.getCell(`E${rowIndex}`).alignment = { horizontal: 'center', vertical: 'middle' };

        rowIndex++; // Move to the next row for employee data
        // Inner loop for each employee in data[i]
        for (var j = 0; j < data[i].length; j++) {
            // Access the individual employee's data
            let employee = data[i][j];

            // Merge cells and insert employee name
            //worksheet.mergeCells(`A${rowIndex}:D${rowIndex + 1}`);
            worksheet.getCell(`A${rowIndex}`).value = employee.employee.replace(/^\d+/, '');
            worksheet.getCell(`A${rowIndex}`).alignment = { horizontal: 'Left', vertical: 'middle' };
            worksheet.getCell(`A${rowIndex}`).font = { size: 10 };

            // Merge cells and insert employee position
            //worksheet.mergeCells(`E${rowIndex}:I${rowIndex + 1}`);
            worksheet.getCell(`B${rowIndex}`).value = employee.position;
            worksheet.getCell(`B${rowIndex}`).alignment = { horizontal: 'Left', vertical: 'middle' };
            worksheet.getCell(`B${rowIndex}`).font = { size: 10 };

            // Merge cells and insert employee inTime
            //worksheet.mergeCells(`J${rowIndex}:K${rowIndex + 1}`);
           
            worksheet.getCell(`C${rowIndex}`).value = employee.inTime;
            worksheet.getCell(`C${rowIndex}`).alignment = { horizontal: 'center', vertical: 'middle' };
            worksheet.getCell(`C${rowIndex}`).font = { size: 10 };
            if (employee.inColor == 'green') {
                worksheet.getCell(`C${rowIndex}`).fill = {
                    type: 'pattern',
                    pattern: 'solid',
                    fgColor: { argb: '92D050' }
                };
            }
            if (employee.inColor == 'red') {
                worksheet.getCell(`C${rowIndex}`).fill = {
                    type: 'pattern',
                    pattern: 'solid',
                    fgColor: { argb: 'FFFF0000' }
                };
            }
            

            // Merge cells and insert employee outTime
            //worksheet.mergeCells(`L${rowIndex}:M${rowIndex + 1}`);
            worksheet.getCell(`D${rowIndex}`).value = employee.outTime;
            worksheet.getCell(`D${rowIndex}`).alignment = { horizontal: 'center', vertical: 'middle' };
            worksheet.getCell(`D${rowIndex}`).font = { size: 10 };
            if (employee.outColor == 'green') {
                worksheet.getCell(`D${rowIndex}`).fill = {
                    type: 'pattern',
                    pattern: 'solid',
                    fgColor: { argb: '92D050' }
                };
            }
            if (employee.outColor == 'red') {
                worksheet.getCell(`D${rowIndex}`).fill = {
                    type: 'pattern',
                    pattern: 'solid',
                    fgColor: { argb: 'FFFF0000' }
                };
            }

            // Merge cells and insert employee date
            //worksheet.mergeCells(`N${rowIndex}:O${rowIndex + 1}`);
            worksheet.getCell(`E${rowIndex}`).value = formatDate(employee.date);
            worksheet.getCell(`E${rowIndex}`).alignment = { horizontal: 'center', vertical: 'middle' };
            worksheet.getCell(`E${rowIndex}`).font = { size: 10 };

            // Move to the next row (increment by 2 because each employee takes up two rows)
            rowIndex += 1;
        }
    }

    worksheet.eachRow({ includeEmpty: true }, function (row, rowNumber) {
        row.eachCell({ includeEmpty: true }, function (cell, colNumber) {
            cell.border = {
                top: { style: 'thin' },
                left: { style: 'thin' },
                bottom: { style: 'thin' },
                right: { style: 'thin' }
            };
        });
    });

    workbook.xlsx.writeBuffer().then(function (buffer) {
        saveAs(new Blob([buffer], { type: "application/octet-stream" }), "საშვების ტაბელი.xlsx");
    });
}


function formatDate(dateString) {
    let date = new Date(dateString); // Convert the string to a Date object

    let day = String(date.getDate()).padStart(2, '0'); // Get the day, with leading zero if needed
    let month = String(date.getMonth() + 1).padStart(2, '0'); // Get the month (Months are zero-indexed, so +1), with leading zero if needed
    let year = date.getFullYear(); // Get the full year

    return `${day}:${month}:${year}`; // Return the formatted date
}

function DisplayTAReports(data)
{
    $("#reporMainPage").hide();
    //alert("bacho"); $('#ShowReport');
    document.getElementById('exportable-table').style.display = 'block';

    var results = $('#exportable-table');
    results.empty(); 
    var hOne = $('<h1>');
    hOne.append('სამუშაო დროის აღრიცხვის ფორმა')

    var hTwo = $('<h2>');
    hTwo.append('შპს "გარდაბნის თბოსადგური"')

    var h4 = $('<h4>');
    var span = $('<span style="color: #007bff">');
    span.append(data[0].unit);
    h4.append('სტრუქტურული ერთეული: ')
    h4.append(span);

    var h42 = $('<h4>');
    var span2 = $('<span style="color: #007bff">');
    span2.append(' 404428071');
    h42.append('საიდენთიფიკაციო კოდი:  ')
    h42.append(span2);
    results.append(hOne);
    results.append(hTwo);
    results.append(h4);
    results.append(h42);


    // Create the table structure dynamically
    var table = $('<table>').addClass('date-section');

    // Create the first row
    var row1 = $('<tr>');
    var td1 = $('<td>').attr('colspan', '4').attr('rowspan', '3').addClass('left').text('შედგენის თარიღი : ' +  data[0].compilationDate);
    var td2 = $('<td>').attr('colspan', '4').addClass('date').text('საანგარიშო პერიოდი');
    row1.append(td1).append(td2);
    table.append(row1);

    // Create the second row
    var row2 = $('<tr>');
    var td3 = $('<td style="background-color: #f2f2f2">').attr('colspan', '2').addClass('center').text('-დან');
    var td4 = $('<td style="background-color: #f2f2f2">').attr('colspan', '2').addClass('center').text('-მდე');
    row2.append(td3).append(td4);
    table.append(row2);

    // Create the third row
    var row3 = $('<tr>');
    var td5 = $('<td>').attr('colspan', '2').addClass('date').text(data[0].datefrom);
    var td6 = $('<td>').attr('colspan', '2').addClass('date').text(data[0].dateTo);
    row3.append(td5).append(td6);
    table.append(row3);

    // Append the table to the results div
    results.append(table);


    // Create the table and append it to the result container
    var table = $('<table>').addClass('table table-bordered'); // You can add more classes like 'table-striped'

    // Create the table header
    var thead = $('<thead>');
    var headerRow1 = $('<tr>');
    headerRow1.append('<th style="background-color: #f2f2f2;" rowspan="2">რიგითი №</th>')
        .append('<th style="background-color: #f2f2f2; " rowspan="2">სახელი გვარი</th>')
        .append('<th style="background-color: #f2f2f2" rowspan="2">თანამდებობა (სპეციალობა)</th>')
        .append('<th style="background-color: #f2f2f2" rowspan="2">ტაბელის ნომერი</th>')
        .append('<th style="background-color: #f2f2f2" colspan="31">' + data[0].unit +'</th>')
        .append('<th style="background-color: #f2f2f2" rowspan="2">სულ ნამუშევარი (საათი)</th>')
        .append('<th style="background-color: #f2f2f2" rowspan="2">სულ ნამუშევარი დღე / ცვლა</th>');

    var headerRow2 = $('<tr>');
    for (var i = 1; i <= data[0].data.length; i++) {
        headerRow2.append('<th>' + i + '</th>');
    }

    // Append the header rows to thead
    thead.append(headerRow1);
    thead.append(headerRow2);
    table.append(thead);

    // Create the table body
    var tbody = $('<tbody>');

    // Data for table rows (replace this with real data if available)

    var employees = [];
    for (var i = 0; i < data.length; i++) {
        
        employees.push({
            id: i + 1, name: data[i].employee.replace(/^\d+/, ''), position: data[i].position, tabelNumber: data[i].tabelNumber, times: data[i].data, totalHours: data[i].fulltimeStr, totalDays: data[i].workingDayes
        })
    }

    // Create rows for each employee
    for (var j = 0; j < employees.length; j++) {
        var emp = employees[j]; // Access each employee object
        var row = $('<tr>'); // Create a new row for each employee
        // Append employee details to the row
        row.append('<td>' + emp.id + '</td>');
        row.append('<td>' + emp.name + '</td>');
        row.append('<td>' + emp.position + '</td>');
        row.append('<td>' + emp.tabelNumber + '</td>'); // Empty field for "ტაბელის ნომერი"

         //Loop over the 'times' array for each employee and set the background color to yellow
        for (var i = 0; i < emp.times.length; i++) {
            var newTd
            if (emp.times[i].vacantion == 'vacantion') {
                newTd = $('<td style="background-color: #974706"></td>');
            }
            if (emp.times[i].inTime !== '' /*&& emp.times[i].vacantion != 'vacantion' */) {
                newTd = $('<td style="background-color: #90ee90"></td>');
            }
            if (emp.times[i].outTime !== '' /*&& emp.times[i].inTime != 'Saturday' */) {
                newTd = $('<td style="background-color: #90ee90"></td>');
            }
            if ((emp.times[i].inTime == '' && emp.times[i].outTime == '') && (emp.times[i].inTime != 'Saturday' || mp.times[i].inTime == 'Sunday') && emp.times[i].vacantion != 'vacantion') {
                newTd = $('<td style="background-color: #ff6961"></td>');
            }
            if ((emp.times[i].inTime != '' || emp.times[i].outTime != '')&&(emp.times[i].inTime == 'Saturday' || emp.times[i].inTime == 'Sunday' | emp.times[i].inTime == 'PublicHolliDay')) {
                newTd = $('<td style="background-color: #ffff00"></td>');
            }
            
            //newTd = $('<td style="background-color: #ffff00"></td>');
              // Create the <td> element
            // Add the 'yellow' class to it
            row.append(newTd);           // Append it to the row
        }

        // Append the total hours and total days columns
        row.append('<td>' + emp.totalHours + '</td>');
        row.append('<td>' + emp.totalDays + '</td>');

        // Append the row to tbody
        tbody.append(row);
    }


    // Append the tbody to the table
    table.append(tbody);

    // Create the table and append it to the result container
    var table1 = $('<table>').addClass('legend-section');

    // First row
    var row1 = $('<tr>');
    row1.append('<td class="left-align">ნამუშევარი</td>');
    row1.append('<td><div class="color-box green"></div> მოსვლა</td>');
    row1.append('<td><div class="color-box red"></div> არმოსვლა</td>');
    row1.append('<td><div class="color-box yellow"></div> უქმე დღე</td>');
    table1.append(row1);

    // Second row
    var row2 = $('<tr>');
    row2.append('<td class="left-align">არგამოცხადების</td>');
    row2.append('<td><div class="color-box blue"></div> თვითიზოლაცია</td>');
    row2.append('<td><div class="color-box gray"></div> დისტანციური</td>');
    row2.append('<td><div class="color-box orange"></div> საავადმყოფო ფურცელი</td>');
    row2.append('<td colspan="2"><div class="color-box yellow-dark"></div> შვებულება</td>');
    table1.append(row2);

    // Third row (signature row)
    var row3 = $('<tr>');
    row3.append('<td style="font-weight: bold;" class="signature-section" colspan="3">ორგანიზაციის/სტრუქტურული ქვედანაყოფის ხელმძღვანელი</td>');
    row3.append('<td style="font-weight: bold; class="signature-section" colspan="3">თანამდებობა</td>');
    row3.append('<td style="font-weight: bold; class="signature-section">ხელმოწერა</td>');
    table1.append(row3);

    // Fourth row (signature details)
    var row4 = $('<tr>');
    row4.append('<td class="signature-section" colspan="3">ავთანდილ შელიავა</td>');
    row4.append('<td class="signature-section" colspan="3">IT სამსახურის უფროსი</td>');
    row4.append('<td class="signature-section"></td>');
    table1.append(row4);


    // Append the complete table to the results div
    results.append(table);
    //results.append(table1);


    var exportButton = $('<button>')
        .attr('id', 'exportToExcelBtn') // Set the id for the button
        .addClass('btn btn-primary') // Add Bootstrap class for styling (or any custom class)
        .text('Export to Excel');

    exportButton.css({
        'padding': '10px 20px',
        'background-color': '#007bff',
        'border': 'none',
        'color': '#fff',
        'cursor': 'pointer',
        'font-size': '16px'
    });

    results.append(exportButton);
   

    $("#exportToExcelBtn").on('click', function () {
        exportToExcel(data);
    });

}


function exportToExcel(data) {
    var workbook = new ExcelJS.Workbook();
    var worksheet = workbook.addWorksheet(data[0].unit);
    worksheet.getColumn(1).width = 5;
    for (let i = 5; i <= 35; i++) { // A to AW (49 columns)
        worksheet.getColumn(i).width = 3; // Approximate 3px width
    }

    for (let i = 2; i < 5; i++) { // A to AW (49 columns)
        worksheet.getColumn(i).width = 15; // Approximate 3px width
    }

    for (let i = 36; i < 38; i++) { // A to AW (49 columns)
        worksheet.getColumn(i).width = 10; // Approximate 3px width
    }

    // Add titles (e.g., h1, h2, etc.)
    worksheet.mergeCells('A1:AK1');
    worksheet.getCell('A1').value = 'სამუშაო დროის აღრიცხვის ფორმა ';
    worksheet.getCell('A1').font = { size: 24, bold: true };
    worksheet.getCell('A1').alignment = { horizontal: 'center', vertical: 'middle' };

    worksheet.mergeCells('A2:AK2');
    worksheet.getCell('A2').value = 'შპს "გარდაბნის თბოსადგური"';
    worksheet.getCell('A2').font = { size: 16, bold: true };
    worksheet.getCell('A2').alignment = { horizontal: 'center', vertical: 'middle' };

    // Add subtitle (structural unit and identification code)
    worksheet.mergeCells('A3:AK3');
    worksheet.getCell('A3').value = 'სტრუქტურული ერთეული: ' + data[0].unit;
    worksheet.getCell('A3').alignment = { horizontal: 'left', vertical: 'middle' };

    worksheet.mergeCells('A4:AK4');
    worksheet.getCell('A4').value = 'საიდენთიფიკაციო კოდი: 404428071';
    worksheet.getCell('A4').alignment = { horizontal: 'left', vertical: 'middle' };

    worksheet.mergeCells('A5:T10');
    worksheet.getCell('A5').value = 'შედგენის თარიღი:' + data[0].compilationDate;
    worksheet.getCell('A5').alignment = { horizontal: 'center', vertical: 'middle' };
    worksheet.getCell('A5').font = { /*color: { argb: 'FFFF00' }, */ bold: true };
    
    worksheet.mergeCells('U5:AK6');
    worksheet.getCell('U5').value = 'საანგარიშო პერიოდი' ;
    worksheet.getCell('U5').alignment = { horizontal: 'center', vertical: 'middle' };
    worksheet.getCell('U5').font = { /*color: { argb: 'FFFF00' }, */ bold: true };

    worksheet.mergeCells('U7:AF8');
    worksheet.getCell('U7').value = 'დან';
    worksheet.getCell('U7').alignment = { horizontal: 'center', vertical: 'middle' };
    worksheet.getCell('U7').font = { /*color: { argb: 'FFFF00' }, */ bold: true };

    worksheet.mergeCells('AG7:AK8');
    worksheet.getCell('AG7').value = 'მდე';
    worksheet.getCell('AG7').alignment = { horizontal: 'center', vertical: 'middle' };
    worksheet.getCell('AG7').font = { /*color: { argb: 'FFFF00' }, */ bold: true };

    worksheet.mergeCells('U9:AF10');
    worksheet.getCell('U9').value = data[0].datefrom;
    worksheet.getCell('U9').alignment = { horizontal: 'center', vertical: 'middle' };
    worksheet.getCell('U9').font = { /*color: { argb: 'FFFF00' }, */ bold: true };

    worksheet.mergeCells('AG9:AK10');
    worksheet.getCell('AG9').value = data[0].dateTo;
    worksheet.getCell('AG9').alignment = { horizontal: 'center', vertical: 'middle' };
    worksheet.getCell('AG9').font = { /*color: { argb: 'FFFF00' }, */ bold: true };

    worksheet.mergeCells('A11:A12');
    worksheet.getCell('A11').value = 'N';
    worksheet.getCell('A11').alignment = { horizontal: 'center', vertical: 'middle' };
    worksheet.getCell('A11').font = { /*color: { argb: 'FFFF00' }, */ bold: true };

    worksheet.mergeCells('B11:B12');
    worksheet.getCell('B11').value = 'საცელი გვარი';
    worksheet.getCell('B11').alignment = { horizontal: 'center', vertical: 'middle' };
    worksheet.getCell('B11').font = { size: 9, bold: true };

    worksheet.mergeCells('C11:C12');
    worksheet.getCell('C11').value = 'თანამდებობა';
    worksheet.getCell('C11').alignment = { horizontal: 'center', vertical: 'middle' };
    worksheet.getCell('C11').font = { size: 9, bold: true };


    worksheet.mergeCells('D11:D12');
    worksheet.getCell('D11').value = 'ტაბელის ნომერი';
    worksheet.getCell('D11').alignment = { horizontal: 'center', vertical: 'middle' };
    worksheet.getCell('D11').font = { size: 9, bold: true };

    worksheet.mergeCells('E11:AI11');
    worksheet.getCell('E11').value = data[0].unit;
    worksheet.getCell('E11').alignment = { horizontal: 'center', vertical: 'middle' };
    worksheet.getCell('E11').font = { /*color: { argb: 'FFFF00' }, */ bold: true };

    let colIdx = 5; // Starting at column E, which is column index 5
    for (let i = 1; i <= data[0].data.length; i++) {
        let cell = worksheet.getCell(12, colIdx); // Get the cell reference
        cell.value = i; // Insert the number (1, 2, ..., 31)
        cell.alignment = { horizontal: 'center', vertical: 'middle' };
        cell.font = { bold: false };

        // Set the background color to light yellow
        if (data[0].data[i - 1].inTime === 'Saturday' || data[0].data[i - 1].inTime === 'Sunday') {
            cell.fill = {
                type: 'pattern',
                pattern: 'solid',
                fgColor: { argb: 'FFFF99' } 
            };
        }
        colIdx++; 
    }


    worksheet.mergeCells('AJ11:AJ12');
    worksheet.getCell('AJ11').value = 'ნამუშევარი საათი';
    worksheet.getCell('AJ11').alignment = {
        horizontal: 'center',
        vertical: 'middle',
        wrapText: true  // Enable wrap text
    };
    worksheet.getCell('AJ11').font = { size: 9, bold: true };

    worksheet.mergeCells('AK11:AK12');
    worksheet.getCell('AK11').value = 'ნამუშევარი დღე / ცვლა';
    worksheet.getCell('AK11').alignment = {
        horizontal: 'center',
        vertical: 'middle',
        wrapText: true  // Enable wrap text
    };
    worksheet.getCell('AK11').font = { size: 9, bold: true };


    let rowIndex = 13;

    for (var i = 0; i < data.length; i++) {
        let row = worksheet.getRow(rowIndex); // Create a new row for each employee

        // Add employee details in columns A, B, C
        row.getCell(1).value = i + 1;
        row.getCell(1).alignment = { horizontal: 'center', vertical: 'middle' };
        row.getCell(1).font = { bold: false };
        worksheet.getRow(rowIndex).height = 30;

        row.getCell(2).value = data[i].employee.replace(/^\d+/, '');  // Column A: Employee name
        row.getCell(2).alignment = {
            horizontal: 'left',
            vertical: 'middle',
            wrapText: true  // Enable wrap text
        };
        row.getCell(2).font = { size: 9, bold: false };
        row.getCell(3).value = data[i].position;
        row.getCell(3).alignment = {
            horizontal: 'left',
            vertical: 'middle',
            wrapText: true  // Enable wrap text
        };
        row.getCell(3).font = { size: 9, bold: false };
        row.getCell(4).value = data[i].tabelNumber;
        row.getCell(4).font = { size: 9, bold: false };// Column C: Tabelis Nomeri or other information

        // Add employee data (like times) in the remaining columns (starting from column D, or 4)
        let colIdx = 5; // Start from column D
        for (var j = 0; j < data[i].data.length; j++) {
            let cell = row.getCell(colIdx);  // Get the current cell
            //cell.value = '';  // Populate data in cells D onwards

            if (data[i].data[j].vacantion == 'vacantion' /*&& (data[i].data[j].inTime !== 'Sunday' || data[i].data[j].inTime !== 'Saturday')*/) {
                cell.fill = {
                    type: 'pattern',
                    pattern: 'solid',
                    fgColor: { argb: '974706' } // Green background
                };
            }

            // Set the background color for the cell (for example, light yellow)
            if (data[i].data[j].inTime !== '' /*&& (data[i].data[j].inTime !== 'Sunday' || data[i].data[j].inTime !== 'Saturday')*/) {
                cell.fill = {
                    type: 'pattern',
                    pattern: 'solid',
                    fgColor: { argb: '92D050' } // Green background
                };
            }

            if (data[i].data[j].outTime !== '' /*&& (data[i].data[j].inTime !== 'Sunday' || data[i].data[j].inTime !== 'Saturday')*/) {
                cell.fill = {
                    type: 'pattern',
                    pattern: 'solid',
                    fgColor: { argb: '92D050' } // Green background
                };
            }

            if ((data[i].data[j].inTime === '' && data[i].data[j].outTime === '') && (data[i].data[j].inTime !== 'Sunday' || data[i].data[j].inTime !== 'Saturday') && data[i].data[j].vacantion != 'vacantion') {
                cell.fill = {
                    type: 'pattern',
                    pattern: 'solid',
                    fgColor: { argb: 'FFFF0000' } // Red background
                };
            }

            if ((data[i].data[j].inTime !== '' || data[i].data[j].outTime !== '')&&(data[i].data[j].inTime === 'Saturday' || data[i].data[j].inTime === 'Sunday' || data[i].data[j].inTime === 'PublicHolliDay')) {
                cell.fill = {
                    type: 'pattern',
                    pattern: 'solid',
                    fgColor: { argb: 'FFFFFF00' } // Light yellow background color
                };
            }
           
            

            colIdx++;  // emp.times[i].inTime !== '' && emp.times[i].inTime != 'Saturday'
        }
        row.getCell(36).value = data[i].fulltimeStr;
        row.getCell(36).alignment = { horizontal: 'center', vertical: 'middle' };
        row.getCell(36).font = { size: 9, bold: false };
        row.getCell(37).value = data[i].workingDayes;
        row.getCell(37).alignment = { horizontal: 'center', vertical: 'middle' };
        row.getCell(37).font = { size: 9, bold: false };
        rowIndex++; // Move to the next row for the next employee
    }

    worksheet.mergeCells(rowIndex + 3, 1, rowIndex + 3, 11);

    // Set the value and alignment for the merged cell
    worksheet.getCell(rowIndex + 3, 1).value = 'ნამუშევარი: ';
    worksheet.getCell(rowIndex + 3, 1).alignment = { horizontal: 'center', vertical: 'middle' };
    worksheet.getCell(rowIndex + 3, 1).font = { bold: true };

    worksheet.getCell(rowIndex + 3, 12).fill = {
        type: 'pattern',
        pattern: 'solid',
        fgColor: { argb: '92D050' } 
    };

    worksheet.mergeCells(rowIndex + 3, 13, rowIndex + 3, 17);
    worksheet.getCell(rowIndex + 3, 13).value = 'მოსვლა ';
    worksheet.getCell(rowIndex + 3, 13).alignment = { horizontal: 'center', vertical: 'middle' };
    worksheet.getCell(rowIndex + 3, 13).font = { bold: false };


    worksheet.getCell(rowIndex + 3, 18).fill = {
        type: 'pattern',
        pattern: 'solid',
        fgColor: { argb: 'FFFF0000' }
    };

    worksheet.mergeCells(rowIndex + 3, 19, rowIndex + 3, 24);
    worksheet.getCell(rowIndex + 3, 19).value = 'არმოსვლა ';
    worksheet.getCell(rowIndex + 3, 19).alignment = { horizontal: 'center', vertical: 'middle' };
    worksheet.getCell(rowIndex + 3, 19).font = { bold: false };

    worksheet.getCell(rowIndex + 3, 25).fill = {
        type: 'pattern',
        pattern: 'solid',
        fgColor: { argb: 'FFFFFF00' }
    };

    worksheet.mergeCells(rowIndex + 3, 26, rowIndex + 3, 31);
    worksheet.getCell(rowIndex + 3, 26).value = 'უქმე დღე ';
    worksheet.getCell(rowIndex + 3, 26).alignment = { horizontal: 'center', vertical: 'middle' };
    worksheet.getCell(rowIndex + 3, 26).font = { bold: false };


    //-----------------------------------------------

    worksheet.mergeCells(rowIndex + 4, 1, rowIndex + 4, 11);
    worksheet.getCell(rowIndex + 4, 1).value = 'არგამოცხადების: ';
    worksheet.getCell(rowIndex + 4, 1).alignment = { horizontal: 'center', vertical: 'middle' };
    worksheet.getCell(rowIndex + 4, 1).font = { bold: true };

    worksheet.getCell(rowIndex + 4, 12).fill = {
        type: 'pattern',
        pattern: 'solid',
        fgColor: { argb: '007BFF' }
    };

    worksheet.mergeCells(rowIndex + 4, 13, rowIndex + 4, 17);
    worksheet.getCell(rowIndex + 4, 13).value = 'თვითიზოლაცია ';
    worksheet.getCell(rowIndex + 4, 13).alignment = { horizontal: 'center', vertical: 'middle' };
    worksheet.getCell(rowIndex + 4, 13).font = { bold: false };

    worksheet.getCell(rowIndex + 4, 18).fill = {
        type: 'pattern',
        pattern: 'solid',
        fgColor: { argb: 'FF808080' }
    };

    worksheet.mergeCells(rowIndex + 4, 19, rowIndex + 4, 24);
    worksheet.getCell(rowIndex + 4, 19).value = 'დისტანციური ';
    worksheet.getCell(rowIndex + 4, 19).alignment = { horizontal: 'center', vertical: 'middle' };
    worksheet.getCell(rowIndex + 4, 19).font = { bold: false };

    worksheet.getCell(rowIndex + 4, 25).fill = {
        type: 'pattern',
        pattern: 'solid',
        fgColor: { argb: '974706' }
    };

    worksheet.mergeCells(rowIndex + 4, 26, rowIndex + 4, 31);
    worksheet.getCell(rowIndex + 4, 26).value = 'შვებულება';
    worksheet.getCell(rowIndex + 4, 26).alignment = { horizontal: 'center', vertical: 'middle' };
    worksheet.getCell(rowIndex + 4, 26).font = { bold: false };


    worksheet.getCell(rowIndex + 4, 32).fill = {
        type: 'pattern',
        pattern: 'solid',
        fgColor: { argb: 'FFD700' }
    };

    worksheet.mergeCells(rowIndex + 4, 33, rowIndex + 4, 37);
    worksheet.getCell(rowIndex + 4, 33).value = 'სავადმყოფო ფურცელი';
    worksheet.getCell(rowIndex + 4, 33).alignment = { horizontal: 'left', vertical: 'middle' };
    worksheet.getCell(rowIndex + 4, 33).font = { bold: false };

    //------------------- second block ----------------------------
    worksheet.mergeCells(rowIndex + 5,1, rowIndex + 6, 11);
    worksheet.getCell(rowIndex + 5, 1).value = 'ორგანიზაციის/სტრუქტურული ქვედანაყოფის ხელმძღვანელი';
    worksheet.getCell(rowIndex + 5, 1).alignment = { horizontal: 'center', vertical: 'middle' };
    worksheet.getCell(rowIndex + 5, 1).font = { bold: true };

    worksheet.mergeCells(rowIndex + 5, 12, rowIndex + 6, 31);
    worksheet.getCell(rowIndex + 5, 12).value = 'თანამდებობა';
    worksheet.getCell(rowIndex + 5, 12).alignment = { horizontal: 'center', vertical: 'middle' };
    worksheet.getCell(rowIndex + 5, 12).font = { bold: true };

    worksheet.mergeCells(rowIndex + 5, 32, rowIndex + 6, 37);
    worksheet.getCell(rowIndex + 5, 32).value = 'ხელმოწერა';
    worksheet.getCell(rowIndex + 5, 32).alignment = { horizontal: 'center', vertical: 'middle' };
    worksheet.getCell(rowIndex + 5, 32).font = { bold: true };



    worksheet.mergeCells(rowIndex + 7, 1, rowIndex + 8, 11);
    worksheet.getCell(rowIndex + 7, 1).value = data[0].employee.replace(/^\d+/, '');
    worksheet.getCell(rowIndex + 7, 1).alignment = { horizontal: 'center', vertical: 'middle' };
    worksheet.getCell(rowIndex + 7, 1).font = { bold: true };

    worksheet.mergeCells(rowIndex + 7, 12, rowIndex + 8, 31);
    worksheet.getCell(rowIndex + 7, 12).value = data[0].position;
    worksheet.getCell(rowIndex + 7, 12).alignment = { horizontal: 'center', vertical: 'middle' };
    worksheet.getCell(rowIndex + 7, 12).font = { bold: true };

    worksheet.mergeCells(rowIndex + 7, 32, rowIndex + 8, 37);
    worksheet.getCell(rowIndex + 7, 32).value = '';
    worksheet.getCell(rowIndex + 7, 32).alignment = { horizontal: 'center', vertical: 'middle' };
    worksheet.getCell(rowIndex + 7, 32).font = { bold: true };
    

    worksheet.eachRow({ includeEmpty: true }, function (row, rowNumber) {
        row.eachCell({ includeEmpty: true }, function (cell, colNumber) {
            cell.border = {
                top: { style: 'thin' },
                left: { style: 'thin' },
                bottom: { style: 'thin' },
                right: { style: 'thin' }
            };
        });
    });

    // Export the Excel file
    workbook.xlsx.writeBuffer().then(function (buffer) {
        saveAs(new Blob([buffer], { type: "application/octet-stream" }), data[0].unit + ".xlsx");
    });
}


