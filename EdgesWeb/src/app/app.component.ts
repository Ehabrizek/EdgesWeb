import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { saveAs } from 'file-saver';
import { tap } from 'rxjs';


@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
    inputFile;
    outputFile;

    constructor(private http: HttpClient) {}

    ngOnInit(): void {}

    fileValidation(event) {
        var file :File = event.target.files[0];
        if (file) {
            var allowedExtensions = /(\.xls|\.xlsx)$/i;
            if (!allowedExtensions.exec(file.name)) {
                alert('File must have extension .xls or .xlsx');
                this.inputFile = null;
                event.target.value = null;
                return false;
            }
            else {
                this.inputFile = file;
                return true;
            }
        }
        event.target.value = null;
        return false;
    }

    process() {
        if (this.inputFile) {
            var sheet = (<HTMLInputElement>document.getElementById('sheet')).value;
            var protein = (<HTMLInputElement>document.getElementById('protein')).value
            var pathwaydesc = (<HTMLInputElement>document.getElementById('pathwaydesc')).value;
            var pathwayid = (<HTMLInputElement>document.getElementById('pathwayid')).value;

            var formData = new FormData();
            formData.append("fileData", this.inputFile);
            formData.append("sheet", sheet);
            formData.append("protein", protein);
            formData.append("pathwaydesc", pathwaydesc);
            formData.append("pathwayid", pathwayid);
            
            this.updateResultDisplay("Processing...")
            this.enableDisableProcessButton(true);
            this.sendPostRequest(formData).subscribe();
        }
        else {
            alert('No files were input!')
        }
    }

    sendPostRequest(formData :FormData) {
        return this.http.post('https://combinationedges.com/api/edges', formData, {responseType: 'text'})
          .pipe(
            tap({
              next: (data) => this.logSuccess(data),
              error: (error) => this.logError(error)
            })
          );
    }

    logSuccess(data) {
        this.outputFile = data;
        this.updateResultDisplay('Success!');
        this.enableDisableProcessButton(false);
    }

    logError(error) {
        this.updateResultDisplay(error.error);
        this.enableDisableProcessButton(false);
    }

    updateResultDisplay(message :string) {
        var resultDisplay = document.getElementById('resultDisplay');
        if (resultDisplay) {
            resultDisplay.innerHTML = message;
        }
        this.showHideDownloadButton(message);
    }

    enableDisableProcessButton(disable :boolean)
    {
        var processButton = <HTMLButtonElement>document.getElementById('processButton');
        if (processButton) {
            processButton.disabled = disable
        }
    }

    showHideDownloadButton(message :string) {
        var downloadButton = document.getElementById('downloadButton');
        if (downloadButton) {
            if (message === 'Success!') {
                downloadButton.style.display = "block";
            }
            else {
                downloadButton.style.display = "none";
            }
        }
    }

    download() {
        if (this.outputFile) {
            var data = new Blob([this.outputFile], { type: 'text/plain' });
            saveAs(data, 'EdgesOutput.csv');
        }
        else {
            alert('No files to download! Try processing first, or retry with valid input!');
        }
    }
}
