device-pda-slot-component-slot-name-cartridge = Cartridge

default-program-name = Program
notekeeper-program-name = Notekeeper
book-writer-program-name = BookWriter
book-writer-new-book = New book
book-writer-back = Back
book-writer-delete = Delete
book-writer-save = Save
book-writer-cancel = Cancel
book-writer-title-placeholder = Book title...
book-writer-desc-placeholder = Short description...
book-writer-cover = Cover:
book-writer-add-page = + Page
book-writer-err-title = Title needs at least 3 characters.
book-writer-err-empty = At least one non-empty page required.
book-writer-err-long = Too long: title up to 64, description up to 140, page up to 12000 chars.
book-writer-save-draft = To drafts
book-writer-publish = Publish
book-writer-draft-tag = [Draft]
book-writer-drafts = Drafts
book-writer-editor-title = New book
book-writer-print = Print
book-writer-edit = Edit
book-writer-print-cooldown = You can only print books once a minute
book-writer-printed-page = Page { $n }
ent-BookPrinted = printed book
ent-BookPrinted-desc = A book printed from a PDA.
book-writer-search-placeholder = Search...
book-writer-genre-all = All genres
book-writer-genre = Genre:
book-writer-variant-archive = Archive
book-writer-variant-scarlet = Scarlet
book-writer-variant-herbal = Herbalist
book-writer-variant-grimoire = Grimoire
book-writer-variant-codex = Codex
book-writer-variant-folio = Folio
book-writer-variant-bestiary = Bestiary
book-writer-variant-atlas = Atlas
book-writer-variant-chronicle = Chronicle
book-writer-variant-songs = Songbook
book-writer-variant-necro = Necronomicon
book-writer-variant-almanac = Almanac
book-writer-genre-other = Other
book-writer-genre-scifi = Sci-Fi
book-writer-genre-horror = Horror
book-writer-genre-detective = Detective
book-writer-genre-romance = Romance
book-writer-genre-guide = Guide
book-writer-genre-history = History
book-writer-genre-poetry = Poetry
book-writer-cover-blue = Blue
book-writer-cover-red = Red
book-writer-cover-green = Green
book-writer-cover-purple = Purple
book-writer-cover-black = Black
book-writer-cover-gold = Gold
book-writer-icon-book = Book
book-writer-icon-eye = Eye
book-writer-icon-skull = Skull
book-writer-icon-stars = Stars
book-writer-icon-temple = Temple
book-writer-icon-diamond = Diamond
book-writer-icon-magic = Magic
book-writer-icon-planet = Planet
nano-task-program-name = NanoTask
news-read-program-name = Station news

crew-manifest-program-name = Crew manifest
crew-manifest-cartridge-loading = Loading ...

net-probe-program-name = NetProbe
net-probe-scan = Scanned {$device}!
net-probe-label-name = Name
net-probe-label-address = Address
net-probe-label-frequency = Frequency
net-probe-label-network = Network

log-probe-program-name = LogProbe
log-probe-scan = Downloaded logs from {$device}!
log-probe-label-time = Time
log-probe-label-accessor = Accessed by
log-probe-label-number = #
log-probe-print-button = Print Logs
log-probe-printout-device = Scanned Device: {$name}
log-probe-printout-header = Latest logs:
log-probe-printout-entry = #{$number} / {$time} / {$accessor}

astro-nav-program-name = AstroNav

med-tek-program-name = MedTek

# NanoTask cartridge

nano-task-ui-heading-high-priority-tasks =
    { $amount ->
        [zero] No High Priority Tasks
        [one] 1 High Priority Task
       *[other] {$amount} High Priority Tasks
    }
nano-task-ui-heading-medium-priority-tasks =
    { $amount ->
        [zero] No Medium Priority Tasks
        [one] 1 Medium Priority Task
       *[other] {$amount} Medium Priority Tasks
    }
nano-task-ui-heading-low-priority-tasks =
    { $amount ->
        [zero] No Low Priority Tasks
        [one] 1 Low Priority Task
       *[other] {$amount} Low Priority Tasks
    }
nano-task-ui-done = Done
nano-task-ui-revert-done = Undo
nano-task-ui-priority-low = Low
nano-task-ui-priority-medium = Medium
nano-task-ui-priority-high = High
nano-task-ui-cancel = Cancel
nano-task-ui-print = Print
nano-task-ui-delete = Delete
nano-task-ui-save = Save
nano-task-ui-new-task = New Task
nano-task-ui-description-label = Description:
nano-task-ui-description-placeholder = Get something important
nano-task-ui-requester-label = Requester:
nano-task-ui-requester-placeholder = John NanoTrasen
nano-task-ui-item-title = Edit Task
nano-task-printed-description = [bold]Description[/bold]: {$description}
nano-task-printed-requester = [bold]Requester[/bold]: {$requester}
nano-task-printed-high-priority = [bold]Priority[/bold]: [color=red]High[/color]
nano-task-printed-medium-priority = [bold]Priority[/bold]: Medium
nano-task-printed-low-priority = [bold]Priority[/bold]: Low

# Wanted list cartridge
wanted-list-program-name = Wanted list
wanted-list-label-no-records = It's all right, cowboy
wanted-list-search-placeholder = Search by name and status

wanted-list-age-label = [color=darkgray]Age:[/color] [color=white]{$age}[/color]
wanted-list-job-label = [color=darkgray]Job:[/color] [color=white]{$job}[/color]
wanted-list-species-label = [color=darkgray]Species:[/color] [color=white]{$species}[/color]
wanted-list-gender-label = [color=darkgray]Gender:[/color] [color=white]{$gender}[/color]

wanted-list-reason-label = [color=darkgray]Reason:[/color] [color=white]{$reason}[/color]
wanted-list-unknown-reason-label = unknown reason

wanted-list-initiator-label = [color=darkgray]Initiator:[/color] [color=white]{$initiator}[/color]
wanted-list-unknown-initiator-label = unknown initiator

wanted-list-status-label = [color=darkgray]status:[/color] {$status ->
        [suspected] [color=yellow]suspected[/color]
        [wanted] [color=red]wanted[/color]
        [detained] [color=#b18644]detained[/color]
        [paroled] [color=green]paroled[/color]
        [discharged] [color=green]discharged[/color]
        [hostile] [color=darkred]hostile[/color]
        [eliminated] [color=gray]eliminated[/color]
        *[other] none
    }

wanted-list-history-table-time-col = Time
wanted-list-history-table-reason-col = Crime
wanted-list-history-table-initiator-col = Initiator
