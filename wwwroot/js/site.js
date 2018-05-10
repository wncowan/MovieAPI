// Write your JavaScript code.
$(document).ready(function(){
    $("#error").hide();
    $("#searchBtn").click(function(e){
        console.log("hit search btn")
        e.preventDefault();
        // console.log($('#searchForm').serializeObject());

        var searchObj = {
            search: $("#searchValue").val()
        }

        $.get("/search", searchObj, function(data){
            $("#searchForm").trigger("reset");
            if(data.error){
                $("#error").show();
                $("#error").html(`${data.error}`);
            } else {
                $("#error").hide()
                displayResults(data);
            }
        });
    });
});

function displayResults(data){
    console.log(data)
    let movieList = "";
    data.forEach(movie => {
        movieList += `
        <tr>
            <td>${movie["title"]}</td>
            <td>${movie["released"]}</td>
            <td>${movie["rating"]}</td>
        </tr>
        `
    });
    $("tbody").html(movieList);   
}

 //The function is based on http://css-tricks.com/snippets/jquery/serialize-form-to-json/

$.fn.serializeObject = function() {
    var o = {};
    var a = this.serializeArray();
    $.each(a, function() {
        if (o[this.name]) {
            if (!o[this.name].push) {
                o[this.name] = [o[this.name]];
            }
            o[this.name].push(this.value || '');
        } else {
            o[this.name] = this.value || '';
        }
    });
    return o;
};