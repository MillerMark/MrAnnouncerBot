enum LayoutStyle {
	normal = 0,
	bold = 1,
	italic = 2,
	calculated = 3
}

enum Justification {
	left,
	right,
	center
}

class Column {
	width = 0;
	justification: Justification = Justification.left;
	lineWrapData: LineWrapData;

	determineAlignment(cellContents: string): void {
		// Expecting one of these three patterns: ':---', ':----:', or '---:'
		if (cellContents.startsWith(':'))
			if (cellContents.endsWith(':'))
				this.justification = Justification.center;
			else
				this.justification = Justification.left;
		else if (cellContents.endsWith(':'))
			this.justification = Justification.right;
		else
			this.justification = Justification.left;
	}

	adjustWidth(newWidth: number) {
		if (this.width < newWidth)
			this.width = newWidth;
	}

	constructor() {

	}
}

class LayoutDelimiters {
	inPair: boolean;
	needToClose: boolean;
	lastStart: number;
	constructor(public style: LayoutStyle, public startDelimiter: string, public endDelimiter: string, public fontYOffset: number = 0) {

	}
}

class Span {
	constructor(public startOffset: number, public stopOffset: number, public style: LayoutStyle) {

	}
}

class Table {
	// TODO: Consider making cellHorizontalMargin a parameter to the table.
	static readonly cellHorizontalMargin: number = 8;

	columns: Array<Column>;
	private _width = 0;

	get width(): number {
		if (this._width === 0)
			this.calculateTableWidth();
		return this._width;
	}

	set width(newValue: number) {
		this._width = newValue;
	}

	calculateTableWidth(): void {
		let tableWidth = 0;
		this.columns.forEach(function (column: Column) {
			tableWidth += column.width + 2 * Table.cellHorizontalMargin;
		});
		this._width = tableWidth;
	}

	getColumnByIndex(columnIndex: number): Column {
		if (!this.columns)
			this.columns = [];
		while (this.columns.length < columnIndex + 1)
			this.columns.push(new Column());
		return this.columns[columnIndex];
	}

	createColumns(lineData: Array<LineWrapData>, context: CanvasRenderingContext2D) {
		this.columns = [];
		for (let i = 0; i < lineData.length; i++) {
			const lineDataThisLine: LineWrapData = lineData[i];
			this.collectColumnInfo(lineDataThisLine, context);
		}
	}
	collectColumnInfo(lineDataThisLine: LineWrapData, context: CanvasRenderingContext2D): void {
		let columnIndex = 0;
		let line: string = lineDataThisLine.line.trim();
		if (line.startsWith('|'))
			line = line.substr(1);

		let pipePos: number = line.indexOf('|');
		while (pipePos >= 0) {
			const column: Column = this.getColumnByIndex(columnIndex);

			const cellContents: string = line.substr(0, pipePos).trim();
			if (cellContents.indexOf('---') >= 0) {
				column.determineAlignment(cellContents);
			}
			else {
				// Regular contents.
				// TODO: Add formatting support for cells?????
				const cellWidth: number = context.measureText(cellContents).width;
				column.adjustWidth(cellWidth);
			}
			line = line.substr(pipePos + 1);
			pipePos = line.indexOf('|');
			columnIndex++;
		}
	}
	constructor(public id: number) {

	}
}

class ParagraphWrapData {
	lineData: Array<LineWrapData>;
	tables: Array<Table>;
	private _maxTableWidth = 0;

	get maxTableWidth(): number {
		if (this._maxTableWidth === 0)
			this.calculateMaxTableWidth();
		return this._maxTableWidth;
	}

	set maxTableWidth(newValue: number) {
		this._maxTableWidth = newValue;
	}

	calculateMaxTableWidth() {
		let tableWidth = 0;
		for (let i = 0; i < this.tables.length; i++) {
			const thisTable: Table = this.tables[i];
			if (thisTable.width > tableWidth)
				tableWidth = thisTable.width;
		}
		this._maxTableWidth = tableWidth;
	}

	createTables(context: CanvasRenderingContext2D) {
		this.tables = [];

		let tableLines: Array<LineWrapData>;

		let inTable = false;
		let activeTable: Table = null;
		let tableId = 0;

		this.lineData.forEach(function (lineWrapData: LineWrapData) {
			if (!inTable && lineWrapData.inTableRow) {
				inTable = true; // Entered a new table!!!
				activeTable = new Table(tableId);
				this.tables.push(activeTable);
				tableId++;

				tableLines = [];

				tableLines.push(lineWrapData);
				lineWrapData.table = activeTable;
			}
			else if (inTable && lineWrapData.inTableRow) {
				tableLines.push(lineWrapData);
				lineWrapData.table = activeTable;
			}
			else if (inTable) {
				inTable = false;
				activeTable.createColumns(tableLines, context);
				tableLines = [];
			}
		}, this);

		if (inTable) {
			activeTable.createColumns(tableLines, context);
		}
	}

	constructor() {

	}
}

class LineWrapData {
	table: Table = null;
	constructor(public line: string, public allSpans: Array<Span>, public width: number, public indent: number = 0, public isBullet: boolean = false, public inTableRow: boolean = false) {

	}
}

class WordRenderer {
	activeStyle: LayoutStyle;

	constructor(browserIsObs: boolean) {
		const emphasisFontYOffsetForChrome = -2;
		const emphasisFontYOffsetForObs = -3;
		const emphasisLineYOffsetForChrome = 0;
		const emphasisLineYOffsetForObs = 2;


		if (browserIsObs) {
			this.calculatedFontYOffset = emphasisFontYOffsetForObs;
			this.underlineOffset = emphasisLineYOffsetForObs;
		}
		else {
			this.calculatedFontYOffset = emphasisFontYOffsetForChrome;
			this.underlineOffset = emphasisLineYOffsetForChrome;
		}
	}

	// TODO: rename "detail" to "body"
	fontSize = 18;
	fontName = 'Arial';
	textColor = '#000000';
	tableLineColor = '#CCCCCC';
	emphasisColor = '#800000';
	bulletColor = '#5b3c35';
	emphasisFontHeightIncrease = 3;
	emphasisFontStyleAscender = 19;
	underlineOffset = 0;
	calculatedFontYOffset = 0;
	bulletIndent = 8;

	public setDetailFontNormal(context: CanvasRenderingContext2D) {
		context.font = `${this.fontSize}px ${this.fontName}`;
	}

	public setDetailFontBold(context: CanvasRenderingContext2D) {
		context.font = `700 ${this.fontSize}px ${this.fontName}`;
	}

	public setDetailFontItalic(context: CanvasRenderingContext2D) {
		context.font = `700 italic ${this.fontSize}px ${this.fontName}`;
	}

	public setInlineDetailFontCalculated(context: CanvasRenderingContext2D) {
		context.font = `700 ${this.fontSize + this.emphasisFontHeightIncrease}px ${this.fontName}`;
	}

	// TODO: Consider splitting setActiveStyle between setting font sizes and fillStyle.
	setActiveStyle(context: CanvasRenderingContext2D, style: LayoutStyle): void {
		this.activeStyle = style;
		switch (style) {
			case LayoutStyle.bold:
				this.setDetailFontBold(context);
				context.fillStyle = this.textColor;
				return;
			case LayoutStyle.italic:
				this.setDetailFontItalic(context);
				context.fillStyle = this.textColor;
				return;
			case LayoutStyle.calculated:  // Larger, dark red font.
				this.setInlineDetailFontCalculated(context);
				context.fillStyle = this.emphasisColor;
				return;
		}
		context.fillStyle = this.textColor;
		this.setDetailFontNormal(context);
	}

	drawTableRow(context: CanvasRenderingContext2D, x: number, y: number, lineData: LineWrapData): void {
		const cells: string[] = lineData.line.trim().split('|').filter(Boolean);
		let offset = 0;
		context.save();
		for (let i = 0; i < cells.length; i++) {
			const cellText: string = cells[i].trim();
			if (!lineData.table.columns) {
				console.error('lineData.table.columns is null!!!');
			}
			const column: Column = lineData.table.columns[i];
			offset += Table.cellHorizontalMargin;
			let textX: number = x + offset;
			if (cellText.indexOf('---') >= 0) {
				// Draw header separator line...
				context.beginPath();
				const lineY: number = y + this.fontSize / 4;
				context.moveTo(textX, lineY);
				context.lineTo(textX + column.width, lineY);
				context.strokeStyle = this.tableLineColor;
				context.globalAlpha = 0.5;
				context.stroke();
				context.globalAlpha = 1;
			}
			else {
				if (column && column.justification === Justification.right) {
					context.textAlign = 'right';
					textX += column.width;
				}
				else if (column && column.justification === Justification.center) {
					context.textAlign = 'center';
					textX += column.width / 2;
				}
				else {
					context.textAlign = 'left';
				}

				context.fillText(cellText, textX, y);
			}

			offset += column.width + Table.cellHorizontalMargin;
		}
		context.restore();
	}

	private drawBullet(context: CanvasRenderingContext2D, x: number, y: number) {
		const bulletRadius: number = this.fontSize / 4;
		context.save();
		context.beginPath();
		context.globalAlpha = 0.75;
		context.fillStyle = this.bulletColor;
		const halfFontHeight: number = this.fontSize / 2;
		context.arc(x + this.bulletIndent - 2 * bulletRadius, y + halfFontHeight, bulletRadius, 0, 2 * Math.PI);
		context.fill();
		context.restore();
	}


	public renderParagraphs(context: CanvasRenderingContext2D, lines: Array<LineWrapData>, x: number, y: number, styleDelimiters: Array<LayoutDelimiters>) {
		for (let i = 0; i < lines.length; i++) {
			const lineData: LineWrapData = lines[i];

			if (lineData.isBullet) {
				this.drawBullet(context, x, y);
			}

			if (lineData.inTableRow) {
				this.setActiveStyle(context, LayoutStyle.normal);
				this.drawTableRow(context, x, y, lineData);
				if (lineData.line.indexOf('---') >= 0) {
					y -= this.fontSize / 2;
				}
			}
			else if (lineData.allSpans.length > 0) {
				let offsetX: number = lineData.indent;
				let lastDrawnStyledOffset = 0;
				for (let spanIndex = 0; spanIndex < lineData.allSpans.length; spanIndex++) {
					const span: Span = lineData.allSpans[spanIndex];

					const onTheVeryLastSpan: boolean = spanIndex === lineData.allSpans.length - 1;

					// Special case (first span):
					if (span.startOffset > lastDrawnStyledOffset) {
						// We have a normal part first
						this.setActiveStyle(context, LayoutStyle.normal);
						const normalPart: string = lineData.line.substr(lastDrawnStyledOffset, span.startOffset - lastDrawnStyledOffset);
						context.fillText(normalPart, x + offsetX, y);
						offsetX += context.measureText(normalPart).width;
					}
					// Draw the styled part...
					const styledPart: string = lineData.line.substr(span.startOffset, span.stopOffset - span.startOffset);
					this.setActiveStyle(context, span.style);

					const wordTop: number = y + WordWrapper.getStyleDelimeters(span.style, styleDelimiters).fontYOffset;
					const styledPartWidth: number = context.measureText(styledPart).width;

					if (span.style === LayoutStyle.calculated) { // Underline this.
						let wordStartX: number = x + offsetX;
						const wordEndX: number = wordStartX + styledPartWidth;
						if (styledPart.startsWith(' ')) {
							wordStartX += context.measureText(' ').width;
						}
						context.beginPath();
						const lineY: number = wordTop + this.emphasisFontStyleAscender + this.underlineOffset;
						context.lineWidth = 2;
						context.strokeStyle = '#3e6bb8';
						context.moveTo(wordStartX, lineY);
						context.lineTo(wordEndX, lineY);
						context.stroke();
					}

					context.fillText(styledPart, x + offsetX, wordTop);
					offsetX += styledPartWidth;

					lastDrawnStyledOffset = span.stopOffset;

					// Special case (last span):
					if (onTheVeryLastSpan && span.stopOffset < lineData.line.length) {
						// We have a normal ending part
						const endStart: number = span.stopOffset;
						const normalPart: string = lineData.line.substr(endStart, lineData.line.length - endStart);
						this.setActiveStyle(context, LayoutStyle.normal);
						context.fillText(normalPart, x + offsetX, y);
						offsetX += context.measureText(normalPart).width;
					}
				}
			}
			else {
				this.setActiveStyle(context, LayoutStyle.normal);
				context.fillText(lineData.line, x + lineData.indent, y);
			}
			y += this.fontSize;
		}
		return y;
	}
}


class WordWrapper {

	constructor() {

	}

	public getWordWrappedLines(context: CanvasRenderingContext2D, text: string, maxScaledWidth: number, styleDelimiters: Array<LayoutDelimiters>, wordRenderer: WordRenderer): Array<LineWrapData> {
		const words = text.split(' ');
		const lines: Array<LineWrapData> = [];

		let currentLine = '';

		let allSpans: Array<Span> = [];
		let isBullet = false;
		let indent = 0;
		let inTableRow: boolean;
		let lineWidth = 0;
		let activeStyle: LayoutStyle = LayoutStyle.normal;

		let lastPart = '';

		for (let i = 0; i < words.length; i++) {
			let word = words[i];
			if (i === 0) {
				if (word === '*') {
					isBullet = true;
					indent = wordRenderer.bulletIndent;
					continue;
				}
				if (word.startsWith('|')) {
					word = word.substr(1);
					inTableRow = true;
					currentLine = text;
					lineWidth = -1;
					break; // No support for formatting inside tables as long as we are breaking here.
					// We could continue to collect formatting, but NOT check for word wrap (set wordWrappingThisLine to false).
				}
			}

			let firstPart = '';
			lastPart = '';

			styleDelimiters.forEach(function (styleDelimeters: LayoutDelimiters) {
				styleDelimeters.needToClose = false;
				if (!styleDelimeters.inPair) {
					const delimiterStartIndex: number = word.indexOf(styleDelimeters.startDelimiter);
					if (delimiterStartIndex >= 0) {
						firstPart = word.substr(0, delimiterStartIndex);
						word = word.substr(delimiterStartIndex + styleDelimeters.startDelimiter.length);
						let startOffset: number = firstPart.length;
						if (startOffset > 0)
							startOffset++;
						styleDelimeters.lastStart = currentLine.length + startOffset;
						styleDelimeters.inPair = true;
						activeStyle = styleDelimeters.style;
					}
				}

				//if (word.startsWith(styleDelimeters.startDelimiter)) {
				//	word = word.substr(styleDelimeters.startDelimiter.length);
				//	styleDelimeters.lastStart = currentLine.length;
				//	styleDelimeters.inPair = true;
				//	activeStyle = styleDelimeters.style;
				//}

				// Catches **this bold**: delimeter (where ending bold delimiter is not followed by a space).
				const endDelimeterIndex = word.indexOf(styleDelimeters.endDelimiter);
				if (endDelimeterIndex > 0) {
					lastPart = word.substr(endDelimeterIndex + styleDelimeters.endDelimiter.length, word.length - endDelimeterIndex);
					word = word.substr(0, endDelimeterIndex);
					styleDelimeters.needToClose = true;
				}
			}, this);


			let thisWord = '';
			if (currentLine)
				thisWord = ' ' + word;
			else {
				thisWord = word;
				lineWidth = indent;
			}

			let firstPartWidth = 0; // e.g., a "(" in "(**"
			if (firstPart) {
				firstPartWidth = context.measureText(firstPart).width;
			}

			wordRenderer.setActiveStyle(context, activeStyle);
			const thisWordWidth = context.measureText(thisWord).width;

			if (lineWidth + thisWordWidth + firstPartWidth < maxScaledWidth - indent) {  // Words still fit on this line.
				if (currentLine) {
					currentLine += ' ' + firstPart + word + lastPart;
					lineWidth += firstPartWidth + thisWordWidth;
				}
				else {
					currentLine = firstPart + word + lastPart;
					lineWidth = indent + firstPartWidth + thisWordWidth;
				}
			}
			else {  // We are wrapping to the next line!!!
				styleDelimiters.forEach(function (styleDelimeters: LayoutDelimiters) {
					if (styleDelimeters.inPair) {
						allSpans.push(new Span(styleDelimeters.lastStart, currentLine.length, styleDelimeters.style));
						styleDelimeters.lastStart = firstPart.length; // Still in the pair!!!
						//styleDelimeters.inPair = false;  // Looks wrong to me on review.
					}
				}, this);

				lines.push(new LineWrapData(currentLine, allSpans, lineWidth, indent, isBullet));
				isBullet = false;
				currentLine = firstPart + word + lastPart;
				lineWidth = thisWordWidth;
				allSpans = [];

				if (lastPart) {
					styleDelimiters.forEach(function (styleDelimeters: LayoutDelimiters) {
						if (styleDelimeters.inPair) {
							allSpans.push(new Span(styleDelimeters.lastStart, word.length, styleDelimeters.style));
							styleDelimeters.lastStart = -1;
							styleDelimeters.inPair = false;
							activeStyle = LayoutStyle.normal;
							styleDelimeters.needToClose = false;
						}
					}, this);
				}

			}

			styleDelimiters.forEach(function (styleDelimeters: LayoutDelimiters) {
				if (styleDelimeters.needToClose) {
					allSpans.push(new Span(styleDelimeters.lastStart, currentLine.length - lastPart.length, styleDelimeters.style));
					styleDelimeters.lastStart = -1;
					styleDelimeters.inPair = false;
					activeStyle = LayoutStyle.normal;
					styleDelimeters.needToClose = false;
				}
			}, this);
		}

		// One last check to see if any styles are left hanging and need closure...
		styleDelimiters.forEach(function (styleDelimeters: LayoutDelimiters) {
			if (styleDelimeters.lastStart >= 0) {
				if (currentLine.length > 0) {
					allSpans.push(new Span(styleDelimeters.lastStart, currentLine.length - lastPart.length, styleDelimeters.style));
					styleDelimeters.lastStart = -1;
				}
			}
		}, this);

		lines.push(new LineWrapData(currentLine, allSpans, lineWidth, indent, isBullet, inTableRow));
		return lines;
	}

	public static getStyleDelimeters(style: LayoutStyle, styleDelimiters: Array<LayoutDelimiters>): LayoutDelimiters {
		for (let i = 0; i < styleDelimiters.length; i++) {
			if (styleDelimiters[i].style === style)
				return styleDelimiters[i];
		}
		return null;
	}

	public static initializeStyleDelimiters(styleDelimiters: Array<LayoutDelimiters>) {
		styleDelimiters.forEach(function (layoutDelimeters: LayoutDelimiters) {
			layoutDelimeters.inPair = false;
			layoutDelimeters.lastStart = -1;
			layoutDelimeters.needToClose = false;
		});
	}

	public getWordWrappedLinesForParagraphs(context: CanvasRenderingContext2D, text: string, maxScaledWidth: number, styleDelimiters: Array<LayoutDelimiters>, wordRenderer: WordRenderer): ParagraphWrapData {
		WordWrapper.initializeStyleDelimiters(styleDelimiters);

		const lines: Array<LineWrapData> = text.split("\n").map(para => this.getWordWrappedLines(context, para, maxScaledWidth,
			styleDelimiters, wordRenderer)).reduce((a, b) => a.concat(b), []);
		const paragraphWrapData: ParagraphWrapData = new ParagraphWrapData();
		paragraphWrapData.lineData = lines;
		paragraphWrapData.createTables(context);
		return paragraphWrapData;
	}
}