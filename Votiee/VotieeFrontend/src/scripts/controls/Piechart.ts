//http://jsfiddle.net/ragingsquirrel3/qkHK6/ Source-code used for the intial creation of the piechart

//both components needs to be created when using the piechart
//var pieChart = React.createFactory(SharedComponents.Piechart);
//var legendComponent = React.createFactory(SharedComponents.Legend);
const greyColor = "#C8C8C8";
var ALPHABET = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

var ID_LENGTH = 8;

module SharedComponents {

    export interface PiechartProps {
        height: number;
        width: number;
        answerResults?: Models.AnswerResultViewModel[];
        showLegends?: boolean;
        sliceText: (d: Models.AnswerResultViewModel, i?: number) => {};
    }

    export interface PiechartState {
        piechartUUID: any;
    }

    export class Piechart extends React.Component<PiechartProps, PiechartState> {

        state: PiechartState = {
            piechartUUID: ""
        };

        componentWillMount() {
            this.setState({ piechartUUID: this.generateUUID() });
        }

        componentDidMount() {
            //Only create the prichart if there is any votes
            if (_.any(this.props.answerResults, x => x.VoteCount > 0)) {
                this.createPieChart(this.props);
            } else {
                this.createPieChart(this.props, true);
            }
        }

        componentDidUpdate(prevProps: PiechartProps) {
            //Update the piechart after render
            if (!_.isEqual(this.props, prevProps) && _.any(this.props.answerResults, x => x.VoteCount > 0)) {
                $(`.${this.state.piechartUUID} > svg`).remove();
                this.createPieChart(this.props);
            }
        }

        render() {
            return div({ className: "piechart" },
                div({ className: "piechart-container" },
                    div({ id: "chart", className: this.state.piechartUUID }),
                    div({ className: "legends" },
                        this.props.showLegends
                        ? _.map(this.props.answerResults, (x, i) => legendComponent({ text: x.AnswerText, color: colors[i] }))
                        : null)));
        }

        createPieChart = (data: PiechartProps, greyDefaultCircle: boolean = false) => {
            var voteCountIsApplied = false;
            var r = data.height / 2;
            const vis = d3.select(`.${this.state.piechartUUID}`).append("svg:svg").data([data.answerResults]).attr("width", data.width).attr("height", data.height).append("svg:g").attr("transform", `translate(${r},${r})`);
            const pie = d3.layout.pie().value(d => {
                if (!greyDefaultCircle) {
                    return (<any>d).VoteCount;
                } else if (!voteCountIsApplied) {
                    voteCountIsApplied = true;
                    return 1;
                }

                return 0;
            }); // declare an arc generator function
            var arc = d3.svg.arc().outerRadius(r);

            // select paths, use arc generator to draw
            const arcs = vis.selectAll("g.slice").data(pie).enter().append("svg:g").attr("class", "slice");
            arcs.append("svg:path")
                .attr("fill", (d, i) => {
                    if (!greyDefaultCircle) {
                        return colors[i];
                    } else {
                        return greyColor;
                    }
                })
                .attr("d", d => arc((<any>d)));

            // add the text
            arcs.append("svg:text").attr("transform", (d: any) => {
                d.innerRadius = r/2;
                d.outerRadius = r;
                return `translate(${arc.centroid(d)})`;
            }).attr("text-anchor", "middle").text((d: any, i: number) => {
                if (d.data.VoteCount > 0)
                    return (<any>data.sliceText(d.data, i));
                else {
                    return "";
                }
            });
        };

        generateUUID() {
            let rtn = "";
            for (let i = 0; i < ID_LENGTH; i++) {
                rtn += ALPHABET.charAt(Math.floor(Math.random() * ALPHABET.length));
            }
            return rtn;
        }
    }

    export interface LegendProps {
        color: string;
        text: string;
    }

    export class Legend extends React.Component<LegendProps, {}> {
        render() {
            return div({ className: "legend" },
                div({ className: "legend-color", style: { backgroundColor: this.props.color } }),
                div({ className: "text" }, this.props.text));
        }

    }
}