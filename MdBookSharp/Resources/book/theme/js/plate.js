window.onload = ()=>{
var plates = document.getElementsByTagName("plate");
var links = [];
for (var i = 0; i < plates.length; i++) {
    var traitDiv = document.createElement("div");
    traitDiv.classList.add("trait");
    traitDiv.innerHTML = `<div class="trait">
    <div class="wowhead-tooltip">
      <table class="wrap-table">
          <tbody>
            <tr>
                <td class="wow-tp-td-content">
                  <table class="wow-tp-content-title-table" style="width: 100%;">
                      <tbody>
                        <tr>
                            <td>
    <div class="wow-icon" style="float:left">
      <div class="iconlarge" data-env="live" data-tree="live" data-game="wow">    
        <ins style="background-image: url('../images/icons/${plates[i].getAttribute("img")}');" class="">
        </ins>  
        <del>  
        </del>
      </div>
    </div>
                              <span class="whtt-name"><b class="whtt-name">${plates[i].getAttribute("name")}</b></a>
                              <div class="subtype">${plates[i].getAttribute("subtype")}</div>
                            </td>
                        </tr>
                      </tbody>
                  </table>
                  <table class="wow-tp-content-description-table">
                      <tbody>
                        <tr>
                            <td>
                              <div class="text">
                                ${plates[i].innerHTML} <br>
                              </div>
                            </td>
                        </tr>
                      </tbody>
                  </table>
                </td>
                <th class="wow-tp-th-content"></th>
            </tr>
            <tr class="wow-tp-bottom-tr">
                <th class="wow-tp-bottom" style="background-position: bottom left"></th>
                <th class="wow-tp-bottom-corner" style="background-position: bottom right"></th>
            </tr>
          </tbody>
      </table>
    </div>
  </div>`
  let plate = plates[i];
  links.push({ plate, traitDiv});
}
links.forEach(link=>{
  link.plate.replaceWith(link.traitDiv);
});
}