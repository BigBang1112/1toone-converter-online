let dotNetObjRef;
let tmp = 0;

const dragenter = (e) => {
    tmp++;
    e.currentTarget.classList.add('dragover', 'overflow');
};

const dragleave = (e) => {
    tmp--;
    if (tmp == 0) {
        e.currentTarget.classList.remove('dragover');
    }

    let form = e.currentTarget;

    setTimeout(() => {
        if (tmp == 0) form.classList.remove('overflow');
    }, 150);
};

const getFiles = async (fileList) => {
    for (let i = 0; i < fileList.length; i++) {
        console.log("ok");
        const item = fileList.item(i);
        console.log("ok2");

        const fileData = new Uint8Array(await item.arrayBuffer());

        if (fileData[0] != 71 && fileData[1] != 66 && fileData[2] != 88) {
            // not a GBX file
            continue;
        }

        const version = fileData[3] | (fileData[4] << 8);

        if (version < 0 || version > 10) {
            // corrupted GBX
            continue;
        }

        const classIdOffset = version >= 4 ? 9 : 8;
        const classId = fileData[classIdOffset]
            | (fileData[classIdOffset + 1] << 8)
            | (fileData[classIdOffset + 2] << 16)
            | (fileData[classIdOffset + 3] << 24);

        if (classId != 0x03043000 && classId != 0x24003000 && classId != 0x21080000) {
            // not a map file
            continue;
        }

        await dotNetObjRef.invokeMethodAsync('OnUploadAsync', item.name, item.size, item.lastModifiedDate, fileData);
    }
};

function start(objRef) {
    dotNetObjRef = objRef;
    
    const form = document.getElementById('filedrop');
    const input = document.getElementById('file');
    
    [
        'drag',
        'dragstart',
        'dragend',
        'dragover',
        'dragenter',
        'dragleave',
        'drop',
    ].forEach((e) =>
        form.addEventListener(e, (e) => {
            e.preventDefault();
            e.stopPropagation();
        })
    );

    [('cldragoverick', 'dragenter')].forEach((e) =>
        form.addEventListener(e, dragenter)
    );

    ['dragleave', 'dragend', 'drop'].forEach((e) =>
        form.addEventListener(e, dragleave)
    );

    form.addEventListener('drop', (e) => getFiles(e.dataTransfer.files));
    input.addEventListener('change', async (e) => {
        await getFiles(e.target.files);
        e.target.value = "";
    });
}