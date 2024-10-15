  let messageCount = 0;
  function createMessage(type,icon,content){
    let message = ``;
    const messageShowcase = $('.message-wrapper');
      message = `
        <div class="dm-pop-message rounded-pill message-${type} message-${messageCount}">
            <span class="dm-pop-message__icon">
                <i class="fa fa-${icon}"></i>
            </span>
            <span class="dm-pop-message__text"><p>${content}</p></span>
        </div>
      `;
    messageShowcase.append(message);
    messageCount++;
  }
  function showMessage(type,icon,content){
    createMessage(type,icon,content);
    let thisMessage = messageCount - 1;
    setTimeout(function(){
      $(document).find(".message-"+thisMessage).remove();
    },6000);
  }
